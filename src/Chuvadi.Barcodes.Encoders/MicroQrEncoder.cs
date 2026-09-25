// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — Micro QR encoder

using System;
using System.Collections.Generic;
using Chuvadi.Barcodes.Common;
using Chuvadi.Barcodes.Encoders.Qr;

namespace Chuvadi.Barcodes.Encoders;

/// <summary>Encodes text as Micro QR symbols (M1–M4). Micro QR has no ECI, GS1 or structured append.</summary>
public static class MicroQrEncoder
{
    private static readonly ReedSolomonEncoder Rs = new(GaloisField.QrCode256);

    /// <summary>Encodes <paramref name="text"/> into the smallest Micro QR symbol that fits.</summary>
    /// <param name="text">The data.</param>
    /// <param name="options">Options, or <see langword="null"/> for defaults.</param>
    /// <returns>The symbol.</returns>
    /// <exception cref="BarcodeEncodingException">The data does not fit or contains unsupported characters.</exception>
    public static QrSymbol Encode(string text, MicroQrEncodeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        MicroQrEncodeOptions opts = options ?? new MicroQrEncodeOptions();
        if (opts.MinVersion is < 1 or > 4 || opts.MaxVersion is < 1 or > 4 || opts.MinVersion > opts.MaxVersion)
        {
            throw new ArgumentException("Versions must satisfy 1 <= MinVersion <= MaxVersion <= 4.", nameof(options));
        }

        if (opts.Mask is < 0 or > 3)
        {
            throw new ArgumentException("Mask must be 0–3 or null.", nameof(options));
        }

        if (opts.ErrorCorrection == QrErrorCorrectionLevel.H)
        {
            throw new BarcodeEncodingException("Micro QR does not support error correction level H.");
        }

        QrCharacterSet characterSet = opts.CharacterSet == QrCharacterSet.Auto ? QrCharacterSet.Latin1 : opts.CharacterSet;
        QrCharacter[] characters = QrTextAnalyzer.Analyze(text, characterSet, gs1: false);
        bool kanjiAllowed = characterSet == QrCharacterSet.ShiftJis;

        for (int version = opts.MinVersion; version <= opts.MaxVersion; version++)
        {
            int symbolNumber = QrTables.MicroSymbolNumber(version, opts.ErrorCorrection);
            if (symbolNumber < 0)
            {
                continue;
            }

            int[] headers = new int[4];
            for (int m = 0; m < 4; m++)
            {
                int cc = QrTables.MicroCharCountBits((QrMode)m, version);
                bool allowed = cc > 0 && (m != (int)QrMode.Kanji || kanjiAllowed);
                headers[m] = allowed ? version - 1 + cc : 0;
            }

            List<QrSegment>? segments = QrSegmenter.Segment(characters, headers);
            if (segments is null)
            {
                continue;
            }

            int capacity = QrTables.MicroDataBitCount(symbolNumber);
            int used = 0;
            bool countsFit = true;
            foreach (QrSegment s in segments)
            {
                int cc = QrTables.MicroCharCountBits(s.Mode, version);
                if (s.CharacterCount >= 1 << cc)
                {
                    countsFit = false;
                }

                used += version - 1 + cc + s.DataBitLength;
            }

            if (!countsFit || used > capacity)
            {
                continue;
            }

            BitBuffer bits = new();
            foreach (QrSegment s in segments)
            {
                bits.Append((int)s.Mode, version - 1);
                bits.Append(s.CharacterCount, QrTables.MicroCharCountBits(s.Mode, version));
                s.WriteData(bits);
            }

            bool[] allBits = Finish(bits, version, symbolNumber, capacity);
            BitMatrix matrix = MicroQrMatrixBuilder.Build(version, symbolNumber, allBits, opts.Mask, out int mask);
            return new QrSymbol(BarcodeFormat.MicroQrCode, version, opts.ErrorCorrection, mask, matrix, -1, 0);
        }

        throw new BarcodeEncodingException(
            $"Data does not fit in a Micro QR symbol of versions M{opts.MinVersion}–M{opts.MaxVersion} at level {opts.ErrorCorrection}.");
    }

    private static bool[] Finish(BitBuffer bits, int version, int symbolNumber, int capacity)
    {
        bits.Append(0, Math.Min(QrTables.MicroTerminatorBits(version), capacity - bits.Length));
        int fullCodewordBits = capacity - (capacity % 8);
        if (bits.Length < fullCodewordBits)
        {
            bits.Append(0, (8 - (bits.Length % 8)) % 8);
            for (int pad = 0xEC; bits.Length < fullCodewordBits; pad ^= 0xEC ^ 0x11)
            {
                bits.Append(pad, 8);
            }
        }

        bits.Append(0, capacity - bits.Length);

        byte[] data = bits.ToBytes(); // a final 4-bit codeword becomes nibble << 4
        byte[] ecc = Rs.Encode(data, QrTables.MicroEccCount(symbolNumber));

        bool[] result = new bool[capacity + (ecc.Length * 8)];
        for (int i = 0; i < capacity; i++)
        {
            result[i] = bits[i];
        }

        for (int i = 0; i < ecc.Length * 8; i++)
        {
            result[capacity + i] = ((ecc[i >> 3] >> (7 - (i & 7))) & 1) != 0;
        }

        return result;
    }
}
