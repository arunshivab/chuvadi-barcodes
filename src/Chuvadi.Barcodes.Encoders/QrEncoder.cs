// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR Code Model 2 encoder

using System;
using System.Collections.Generic;
using Chuvadi.Barcodes.Common;
using Chuvadi.Barcodes.Encoders.Qr;

namespace Chuvadi.Barcodes.Encoders;

/// <summary>
/// Encodes text as QR Code Model 2 symbols (versions 1–40), choosing modes optimally,
/// with optional ECI, GS1 (FNC1) and structured append.
/// </summary>
public static class QrEncoder
{
    private const int EciUtf8 = 26;
    private static readonly ReedSolomonEncoder Rs = new(GaloisField.QrCode256);

    /// <summary>Encodes <paramref name="text"/> into a single QR Code symbol.</summary>
    /// <param name="text">The data. May be empty.</param>
    /// <param name="options">Options, or <see langword="null"/> for defaults.</param>
    /// <returns>The symbol.</returns>
    /// <exception cref="BarcodeEncodingException">The data does not fit or contains unsupported characters.</exception>
    public static QrSymbol Encode(string text, QrEncodeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        QrEncodeOptions opts = options ?? new QrEncodeOptions();
        Validate(opts);
        QrCharacterSet characterSet = QrTextAnalyzer.Resolve(text, opts.CharacterSet);
        QrCharacter[] characters = QrTextAnalyzer.Analyze(text, characterSet, opts.Gs1);
        return EncodeCharacters(characters, characterSet, opts, structuredAppend: null);
    }

    /// <summary>
    /// Splits <paramref name="text"/> across <paramref name="symbolCount"/> linked QR Code
    /// symbols (structured append). A reader reassembles them into the original text.
    /// </summary>
    /// <param name="text">The data; must have at least <paramref name="symbolCount"/> characters.</param>
    /// <param name="symbolCount">Number of symbols, 2–16.</param>
    /// <param name="options">Options applied to every symbol, or <see langword="null"/> for defaults.</param>
    /// <returns>The symbols, in sequence order.</returns>
    /// <exception cref="BarcodeEncodingException">A part does not fit or contains unsupported characters.</exception>
    public static IReadOnlyList<QrSymbol> EncodeStructuredAppend(string text, int symbolCount, QrEncodeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfLessThan(symbolCount, 2);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(symbolCount, 16);
        QrEncodeOptions opts = options ?? new QrEncodeOptions();
        Validate(opts);
        QrCharacterSet characterSet = QrTextAnalyzer.Resolve(text, opts.CharacterSet);
        QrCharacter[] characters = QrTextAnalyzer.Analyze(text, characterSet, opts.Gs1);
        if (characters.Length < symbolCount)
        {
            throw new BarcodeEncodingException(
                $"Structured append needs at least one character per symbol ({symbolCount} symbols, {characters.Length} characters).");
        }

        int parity = 0;
        foreach (QrCharacter c in characters)
        {
            foreach (byte b in c.Bytes)
            {
                parity ^= b;
            }
        }

        List<QrSymbol> symbols = new(symbolCount);
        int baseLength = characters.Length / symbolCount;
        int extra = characters.Length % symbolCount;
        int start = 0;
        for (int i = 0; i < symbolCount; i++)
        {
            int length = baseLength + (i < extra ? 1 : 0);
            QrCharacter[] part = characters.AsSpan(start, length).ToArray();
            symbols.Add(EncodeCharacters(part, characterSet, opts, (i, symbolCount, parity)));
            start += length;
        }

        return symbols;
    }

    /// <summary>
    /// Splits text for structured append exactly as <see cref="EncodeStructuredAppend"/> does,
    /// and returns the parts (useful for testing and for printing labels next to each symbol).
    /// </summary>
    /// <param name="text">The data.</param>
    /// <param name="symbolCount">Number of symbols, 2–16.</param>
    /// <returns>The text of each part.</returns>
    public static IReadOnlyList<string> SplitForStructuredAppend(string text, int symbolCount)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfLessThan(symbolCount, 2);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(symbolCount, 16);
        List<string> runes = [];
        foreach (System.Text.Rune rune in text.EnumerateRunes())
        {
            runes.Add(rune.ToString());
        }

        if (runes.Count < symbolCount)
        {
            throw new BarcodeEncodingException("Structured append needs at least one character per symbol.");
        }

        List<string> parts = new(symbolCount);
        int baseLength = runes.Count / symbolCount;
        int extra = runes.Count % symbolCount;
        int start = 0;
        for (int i = 0; i < symbolCount; i++)
        {
            int length = baseLength + (i < extra ? 1 : 0);
            parts.Add(string.Concat(runes.GetRange(start, length)));
            start += length;
        }

        return parts;
    }

    private static void Validate(QrEncodeOptions opts)
    {
        if (opts.MinVersion is < 1 or > 40 || opts.MaxVersion is < 1 or > 40 || opts.MinVersion > opts.MaxVersion)
        {
            throw new ArgumentException("Versions must satisfy 1 <= MinVersion <= MaxVersion <= 40.", nameof(opts));
        }

        if (opts.Mask is < 0 or > 7)
        {
            throw new ArgumentException("Mask must be 0–7 or null.", nameof(opts));
        }
    }

    private static QrSymbol EncodeCharacters(
        QrCharacter[] characters,
        QrCharacterSet characterSet,
        QrEncodeOptions opts,
        (int Index, int Count, int Parity)? structuredAppend)
    {
        bool eci = characterSet == QrCharacterSet.Utf8 && opts.EmitEci;
        int prefixBits = (structuredAppend is null ? 0 : 20) + (eci ? 12 : 0) + (opts.Gs1 ? 4 : 0);
        bool kanjiAllowed = characterSet == QrCharacterSet.ShiftJis;

        QrErrorCorrectionLevel level = opts.ErrorCorrection;
        for (int version = opts.MinVersion; version <= opts.MaxVersion; version++)
        {
            int[] headers = new int[4];
            for (int m = 0; m < 4; m++)
            {
                headers[m] = m == (int)QrMode.Kanji && !kanjiAllowed ? 0 : 4 + QrTables.CharCountBits((QrMode)m, version);
            }

            List<QrSegment>? segments = QrSegmenter.Segment(characters, headers);
            if (segments is null)
            {
                continue;
            }

            int used = prefixBits;
            bool countsFit = true;
            foreach (QrSegment s in segments)
            {
                int ccBits = QrTables.CharCountBits(s.Mode, version);
                if (s.CharacterCount >= 1 << ccBits)
                {
                    countsFit = false;
                }

                used += 4 + ccBits + s.DataBitLength;
            }

            if (!countsFit || used > QrTables.DataCodewords(version, level) * 8)
            {
                continue;
            }

            if (opts.BoostErrorCorrection)
            {
                for (QrErrorCorrectionLevel higher = level + 1; higher <= QrErrorCorrectionLevel.H; higher++)
                {
                    if (used <= QrTables.DataCodewords(version, higher) * 8)
                    {
                        level = higher;
                    }
                }
            }

            BitBuffer bits = new();
            if (structuredAppend is (int index, int count, int parity))
            {
                bits.Append(0b0011, 4);
                bits.Append(index, 4);
                bits.Append(count - 1, 4);
                bits.Append(parity, 8);
            }

            if (eci)
            {
                bits.Append(0b0111, 4);
                bits.Append(EciUtf8, 8);
            }

            if (opts.Gs1)
            {
                bits.Append(0b0101, 4);
            }

            foreach (QrSegment s in segments)
            {
                bits.Append(ModeIndicator(s.Mode), 4);
                bits.Append(s.CharacterCount, QrTables.CharCountBits(s.Mode, version));
                s.WriteData(bits);
            }

            byte[] codewords = AddEccAndInterleave(Finish(bits, QrTables.DataCodewords(version, level)), version, level);
            BitMatrix matrix = QrMatrixBuilder.Build(version, level, codewords, opts.Mask, out int mask);
            return new QrSymbol(
                BarcodeFormat.QrCode,
                version,
                level,
                mask,
                matrix,
                structuredAppend?.Index ?? -1,
                structuredAppend?.Count ?? 0);
        }

        throw new BarcodeEncodingException(
            $"Data does not fit in a QR Code of versions {opts.MinVersion}–{opts.MaxVersion} at level {opts.ErrorCorrection}.");
    }

    private static int ModeIndicator(QrMode mode) => mode switch
    {
        QrMode.Numeric => 0b0001,
        QrMode.Alphanumeric => 0b0010,
        QrMode.Byte => 0b0100,
        _ => 0b1000,
    };

    // Terminator, bit padding and pad codewords.
    private static byte[] Finish(BitBuffer bits, int dataCodewords)
    {
        int capacity = dataCodewords * 8;
        bits.Append(0, Math.Min(4, capacity - bits.Length));
        bits.Append(0, (8 - (bits.Length % 8)) % 8);
        for (int pad = 0xEC; bits.Length < capacity; pad ^= 0xEC ^ 0x11)
        {
            bits.Append(pad, 8);
        }

        return bits.ToBytes();
    }

    private static byte[] AddEccAndInterleave(byte[] data, int version, QrErrorCorrectionLevel level)
    {
        int blockCount = QrTables.BlockCount(version, level);
        int eccLength = QrTables.EccPerBlock(version, level);
        int total = QrTables.TotalCodewords(version);
        int shortBlockCount = blockCount - (total % blockCount);
        int shortBlockLength = total / blockCount;

        List<byte[]> dataBlocks = new(blockCount);
        List<byte[]> eccBlocks = new(blockCount);
        int offset = 0;
        for (int i = 0; i < blockCount; i++)
        {
            int dataLength = shortBlockLength - eccLength + (i < shortBlockCount ? 0 : 1);
            byte[] block = data.AsSpan(offset, dataLength).ToArray();
            offset += dataLength;
            dataBlocks.Add(block);
            eccBlocks.Add(Rs.Encode(block, eccLength));
        }

        byte[] result = new byte[total];
        int k = 0;
        for (int i = 0; i <= shortBlockLength - eccLength; i++)
        {
            foreach (byte[] block in dataBlocks)
            {
                if (i < block.Length)
                {
                    result[k++] = block[i];
                }
            }
        }

        for (int i = 0; i < eccLength; i++)
        {
            foreach (byte[] block in eccBlocks)
            {
                result[k++] = block[i];
            }
        }

        return result;
    }
}
