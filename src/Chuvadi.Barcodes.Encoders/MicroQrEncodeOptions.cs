// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — encoders

namespace Chuvadi.Barcodes.Encoders;

/// <summary>
/// Options for <see cref="MicroQrEncoder"/>. Available levels: M1 detection only (use L);
/// M2 and M3 L or M; M4 L, M or Q. Level H does not exist in Micro QR.
/// </summary>
public sealed class MicroQrEncodeOptions
{
    /// <summary>Error correction level (L, M or Q). Default <see cref="QrErrorCorrectionLevel.L"/>.</summary>
    public QrErrorCorrectionLevel ErrorCorrection { get; init; } = QrErrorCorrectionLevel.L;

    /// <summary>Smallest version (1 = M1 … 4 = M4). Default 1.</summary>
    public int MinVersion { get; init; } = 1;

    /// <summary>Largest version (1 = M1 … 4 = M4). Default 4.</summary>
    public int MaxVersion { get; init; } = 4;

    /// <summary>Mask pattern 0–3, or <see langword="null"/> to pick the best mask. Default null.</summary>
    public int? Mask { get; init; }

    /// <summary>
    /// Character set for byte-mode data: <see cref="QrCharacterSet.Auto"/> and
    /// <see cref="QrCharacterSet.Latin1"/> accept ISO-8859-1 only (Micro QR has no ECI);
    /// <see cref="QrCharacterSet.ShiftJis"/> enables Kanji mode. <see cref="QrCharacterSet.Utf8"/>
    /// writes UTF-8 bytes without an ECI. Default <see cref="QrCharacterSet.Auto"/>.
    /// </summary>
    public QrCharacterSet CharacterSet { get; init; } = QrCharacterSet.Auto;
}
