// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — encoders

namespace Chuvadi.Barcodes.Encoders;

/// <summary>How text is turned into bytes for the byte (and Kanji) modes of QR-family symbols.</summary>
public enum QrCharacterSet
{
    /// <summary>
    /// ISO-8859-1 when every character fits (the QR default, no ECI needed); otherwise UTF-8,
    /// announced with ECI 26 unless <see cref="QrEncodeOptions.EmitEci"/> is off.
    /// </summary>
    Auto,

    /// <summary>ISO-8859-1 (Latin-1). Characters above U+00FF are rejected.</summary>
    Latin1,

    /// <summary>UTF-8, announced with ECI 26 unless <see cref="QrEncodeOptions.EmitEci"/> is off.</summary>
    Utf8,

    /// <summary>
    /// Shift JIS. Double-byte characters use the compact Kanji mode; no ECI is emitted.
    /// Characters outside Shift JIS are rejected.
    /// </summary>
    ShiftJis,
}
