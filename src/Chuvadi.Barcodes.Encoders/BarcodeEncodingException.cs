// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — encoders

using System;

namespace Chuvadi.Barcodes.Encoders;

/// <summary>
/// Thrown when data cannot be encoded: it is too long for the allowed symbol sizes, or it
/// contains characters the symbology or chosen character set cannot represent.
/// </summary>
public sealed class BarcodeEncodingException : Exception
{
    /// <summary>Creates the exception with a default message.</summary>
    public BarcodeEncodingException()
        : base("The data cannot be encoded.")
    {
    }

    /// <summary>Creates the exception with a message.</summary>
    /// <param name="message">What went wrong.</param>
    public BarcodeEncodingException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception with a message and an inner exception.</summary>
    /// <param name="message">What went wrong.</param>
    /// <param name="innerException">The underlying cause.</param>
    public BarcodeEncodingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
