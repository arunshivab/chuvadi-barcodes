// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

namespace Chuvadi.Barcodes;

/// <summary>
/// Error correction level for QR Code, Micro QR Code and rMQR symbols. Higher levels
/// recover from more damage at the cost of a larger symbol.
/// </summary>
public enum QrErrorCorrectionLevel
{
    /// <summary>Level L: recovers roughly 7% of codewords.</summary>
    L,

    /// <summary>Level M: recovers roughly 15% of codewords.</summary>
    M,

    /// <summary>Level Q: recovers roughly 25% of codewords.</summary>
    Q,

    /// <summary>Level H: recovers roughly 30% of codewords.</summary>
    H,
}
