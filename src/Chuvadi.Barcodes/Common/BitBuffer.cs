// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;
using System.Collections.Generic;

namespace Chuvadi.Barcodes.Common;

/// <summary>
/// A growable sequence of bits, appended most-significant bit first. Used to assemble
/// symbol bit streams before they are split into codewords.
/// </summary>
internal sealed class BitBuffer
{
    private readonly List<bool> _bits = [];

    /// <summary>Number of bits appended so far.</summary>
    public int Length => _bits.Count;

    /// <summary>Gets the bit at <paramref name="index"/>.</summary>
    public bool this[int index] => _bits[index];

    /// <summary>Appends the low <paramref name="count"/> bits of <paramref name="value"/>, MSB first.</summary>
    public void Append(int value, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, 31);
        if (count < 31 && (value >> count) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"Value {value} does not fit in {count} bits.");
        }

        for (int i = count - 1; i >= 0; i--)
        {
            _bits.Add(((value >> i) & 1) != 0);
        }
    }

    /// <summary>Appends a single bit.</summary>
    public void AppendBit(bool bit) => _bits.Add(bit);

    /// <summary>Appends every bit of another buffer.</summary>
    public void Append(BitBuffer other)
    {
        ArgumentNullException.ThrowIfNull(other);
        _bits.AddRange(other._bits);
    }

    /// <summary>
    /// Packs the bits into bytes, MSB first. A final partial byte is padded with zero bits.
    /// </summary>
    public byte[] ToBytes()
    {
        byte[] result = new byte[(_bits.Count + 7) / 8];
        for (int i = 0; i < _bits.Count; i++)
        {
            if (_bits[i])
            {
                result[i >> 3] |= (byte)(0x80 >> (i & 7));
            }
        }

        return result;
    }
}
