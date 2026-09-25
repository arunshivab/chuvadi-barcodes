// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — text to QR characters

using System;
using System.Collections.Generic;
using System.Text;

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>Splits text into <see cref="QrCharacter"/>s for the chosen character set.</summary>
internal static class QrTextAnalyzer
{
    /// <summary>The 45 alphanumeric-mode characters, in value order.</summary>
    public const string AlphanumericCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

    private const char GroupSeparator = '\u001D';
    private const int PercentValue = 38;

    private static readonly Lazy<Encoding> ShiftJisEncoding = new(() =>
        CodePagesEncodingProvider.Instance.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
        ?? throw new PlatformNotSupportedException("Shift JIS (code page 932) is not available on this runtime."));

    /// <summary>Resolves <see cref="QrCharacterSet.Auto"/> for QR Code (Latin-1 if possible, else UTF-8).</summary>
    public static QrCharacterSet Resolve(string text, QrCharacterSet requested)
    {
        if (requested != QrCharacterSet.Auto)
        {
            return requested;
        }

        foreach (char c in text)
        {
            if (c > '\u00FF')
            {
                return QrCharacterSet.Utf8;
            }
        }

        return QrCharacterSet.Latin1;
    }

    /// <summary>Analyzes <paramref name="text"/> in an already-resolved character set.</summary>
    /// <exception cref="BarcodeEncodingException">A character cannot be represented.</exception>
    public static QrCharacter[] Analyze(string text, QrCharacterSet characterSet, bool gs1)
    {
        List<QrCharacter> result = new(text.Length);
        int index = 0;
        foreach (Rune rune in text.EnumerateRunes())
        {
            byte[] bytes = Encode(rune, characterSet, index);
            int numeric = rune.Value is >= '0' and <= '9' ? rune.Value - '0' : -1;
            int[] alphanumeric = AlphanumericValuesOf(rune, gs1);
            int kanji = characterSet == QrCharacterSet.ShiftJis ? KanjiValueOf(bytes) : -1;
            result.Add(new QrCharacter(bytes, numeric, alphanumeric, kanji));
            index += rune.Utf16SequenceLength;
        }

        return [.. result];
    }

    private static int[] AlphanumericValuesOf(Rune rune, bool gs1)
    {
        if (gs1 && rune.Value == GroupSeparator)
        {
            return [PercentValue];
        }

        if (rune.Value > 0x7F)
        {
            return [];
        }

        int value = AlphanumericCharset.IndexOf((char)rune.Value, StringComparison.Ordinal);
        if (value < 0)
        {
            return [];
        }

        return gs1 && value == PercentValue ? [PercentValue, PercentValue] : [value];
    }

    private static int KanjiValueOf(byte[] bytes)
    {
        if (bytes.Length != 2)
        {
            return -1;
        }

        int code = (bytes[0] << 8) | bytes[1];
        if (code is >= 0x8140 and <= 0x9FFC)
        {
            code -= 0x8140;
        }
        else if (code is >= 0xE040 and <= 0xEBBF)
        {
            code -= 0xC140;
        }
        else
        {
            return -1;
        }

        return ((code >> 8) * 0xC0) + (code & 0xFF);
    }

    private static byte[] Encode(Rune rune, QrCharacterSet characterSet, int index)
    {
        switch (characterSet)
        {
            case QrCharacterSet.Utf8:
                {
                    byte[] buffer = new byte[rune.Utf8SequenceLength];
                    rune.EncodeToUtf8(buffer);
                    return buffer;
                }

            case QrCharacterSet.ShiftJis:
                {
                    try
                    {
                        return ShiftJisEncoding.Value.GetBytes(rune.ToString());
                    }
                    catch (EncoderFallbackException ex)
                    {
                        throw new BarcodeEncodingException(
                            $"Character U+{rune.Value:X4} at index {index} is not in Shift JIS.", ex);
                    }
                }

            default:
                if (rune.Value > 0xFF)
                {
                    throw new BarcodeEncodingException(
                        $"Character U+{rune.Value:X4} at index {index} is not in ISO-8859-1.");
                }

                return [(byte)rune.Value];
        }
    }
}
