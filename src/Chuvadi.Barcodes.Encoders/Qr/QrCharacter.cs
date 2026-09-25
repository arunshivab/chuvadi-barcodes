// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — per-character analysis for mode selection

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>
/// One input character (Unicode scalar) with everything the segmenter needs: its bytes in
/// the chosen character set and whether it can use the numeric, alphanumeric or Kanji modes.
/// </summary>
internal sealed class QrCharacter
{
    public QrCharacter(byte[] bytes, int numericValue, int[] alphanumericValues, int kanjiValue)
    {
        Bytes = bytes;
        NumericValue = numericValue;
        AlphanumericValues = alphanumericValues;
        KanjiValue = kanjiValue;
    }

    /// <summary>Bytes of this character in byte mode.</summary>
    public byte[] Bytes { get; }

    /// <summary>0–9 when the character is a digit, else -1.</summary>
    public int NumericValue { get; }

    /// <summary>
    /// Alphanumeric-mode values (usually one; two for a literal '%' in GS1 mode), or empty
    /// when the character is not alphanumeric.
    /// </summary>
    public int[] AlphanumericValues { get; }

    /// <summary>13-bit Kanji-mode value, or -1 when Kanji mode cannot be used.</summary>
    public int KanjiValue { get; }
}
