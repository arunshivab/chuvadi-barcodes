// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR data modes

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>QR-family data encoding modes. The order is used as an array index.</summary>
internal enum QrMode
{
    Numeric = 0,
    Alphanumeric = 1,
    Byte = 2,
    Kanji = 3,
}
