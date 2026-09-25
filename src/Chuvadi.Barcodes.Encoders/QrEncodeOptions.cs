// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — encoders

namespace Chuvadi.Barcodes.Encoders;

/// <summary>Options for <see cref="QrEncoder"/>. Defaults produce the smallest valid symbol at level M.</summary>
public sealed class QrEncodeOptions
{
    /// <summary>Error correction level. Default <see cref="QrErrorCorrectionLevel.M"/>.</summary>
    public QrErrorCorrectionLevel ErrorCorrection { get; init; } = QrErrorCorrectionLevel.M;

    /// <summary>Smallest version (1–40) the encoder may choose. Default 1.</summary>
    public int MinVersion { get; init; } = 1;

    /// <summary>Largest version (1–40) the encoder may choose. Default 40.</summary>
    public int MaxVersion { get; init; } = 40;

    /// <summary>Mask pattern 0–7, or <see langword="null"/> to pick the lowest-penalty mask. Default null.</summary>
    public int? Mask { get; init; }

    /// <summary>
    /// When <see langword="true"/>, raises the error correction level as far as possible
    /// without increasing the version. Default <see langword="false"/>.
    /// </summary>
    public bool BoostErrorCorrection { get; init; }

    /// <summary>Character set for byte-mode data. Default <see cref="QrCharacterSet.Auto"/>.</summary>
    public QrCharacterSet CharacterSet { get; init; } = QrCharacterSet.Auto;

    /// <summary>
    /// Whether to emit ECI 26 when the data is UTF-8. Default <see langword="true"/>
    /// (standards-correct). Turn off only for readers that mishandle ECI.
    /// </summary>
    public bool EmitEci { get; init; } = true;

    /// <summary>
    /// Encodes a GS1 QR Code (FNC1 in first position). Separate variable-length element
    /// strings with the GS character U+001D. Default <see langword="false"/>.
    /// </summary>
    public bool Gs1 { get; init; }
}
