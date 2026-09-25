// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — encoders

using System;

namespace Chuvadi.Barcodes.Encoders;

/// <summary>An encoded QR-family symbol: its module grid plus the parameters chosen.</summary>
public sealed class QrSymbol
{
    internal QrSymbol(
        BarcodeFormat format,
        int version,
        QrErrorCorrectionLevel errorCorrection,
        int mask,
        BitMatrix matrix,
        int structuredAppendIndex,
        int structuredAppendCount)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        Format = format;
        Version = version;
        ErrorCorrection = errorCorrection;
        Mask = mask;
        Matrix = matrix;
        StructuredAppendIndex = structuredAppendIndex;
        StructuredAppendCount = structuredAppendCount;
    }

    /// <summary><see cref="BarcodeFormat.QrCode"/> or <see cref="BarcodeFormat.MicroQrCode"/>.</summary>
    public BarcodeFormat Format { get; }

    /// <summary>Symbol version: 1–40 for QR Code, 1–4 (M1–M4) for Micro QR.</summary>
    public int Version { get; }

    /// <summary>Error correction level used.</summary>
    public QrErrorCorrectionLevel ErrorCorrection { get; }

    /// <summary>Mask pattern applied (0–7 for QR Code, 0–3 for Micro QR).</summary>
    public int Mask { get; }

    /// <summary>The modules, without quiet zone.</summary>
    public BitMatrix Matrix { get; }

    /// <summary>Quiet zone width in modules required around the symbol: 4 for QR Code, 2 for Micro QR.</summary>
    public int QuietZone => Format == BarcodeFormat.MicroQrCode ? 2 : 4;

    /// <summary>Zero-based position within a structured-append sequence, or -1 when not part of one.</summary>
    public int StructuredAppendIndex { get; }

    /// <summary>Number of symbols in the structured-append sequence, or 0 when not part of one.</summary>
    public int StructuredAppendCount { get; }
}
