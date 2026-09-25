// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;
using System.Collections.Concurrent;

namespace Chuvadi.Barcodes.Common;

/// <summary>
/// Systematic Reed–Solomon encoder: computes the error-correction codewords appended to a
/// block of data codewords. The generator polynomial is
/// (x - α^b)(x - α^(b+1))…(x - α^(b+n-1)), where b is the field's generator base.
/// </summary>
internal sealed class ReedSolomonEncoder
{
    private readonly GaloisField _field;
    private readonly ConcurrentDictionary<int, int[]> _generators = new();

    /// <summary>Creates an encoder over <paramref name="field"/>.</summary>
    public ReedSolomonEncoder(GaloisField field)
    {
        ArgumentNullException.ThrowIfNull(field);
        _field = field;
    }

    /// <summary>
    /// Computes <paramref name="eccCount"/> error-correction codewords for <paramref name="data"/>.
    /// </summary>
    public int[] Encode(ReadOnlySpan<int> data, int eccCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(eccCount, 1);
        int[] generator = _generators.GetOrAdd(eccCount, BuildGenerator);

        // Polynomial long division of data(x)·x^n by generator(x); the remainder is the ECC.
        int[] remainder = new int[eccCount];
        foreach (int d in data)
        {
            if ((uint)d >= (uint)_field.Size)
            {
                throw new ArgumentOutOfRangeException(nameof(data), $"Codeword {d} is outside the field.");
            }

            int factor = d ^ remainder[0];
            Array.Copy(remainder, 1, remainder, 0, eccCount - 1);
            remainder[eccCount - 1] = 0;
            if (factor != 0)
            {
                for (int i = 0; i < eccCount; i++)
                {
                    remainder[i] ^= _field.Multiply(generator[i + 1], factor);
                }
            }
        }

        return remainder;
    }

    /// <summary>Byte convenience overload.</summary>
    public byte[] Encode(ReadOnlySpan<byte> data, int eccCount)
    {
        int[] ints = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            ints[i] = data[i];
        }

        int[] ecc = Encode(ints, eccCount);
        byte[] result = new byte[ecc.Length];
        for (int i = 0; i < ecc.Length; i++)
        {
            result[i] = (byte)ecc[i];
        }

        return result;
    }

    // Coefficients, highest degree first; generator[0] == 1.
    private int[] BuildGenerator(int degree)
    {
        int[] g = [1];
        for (int i = 0; i < degree; i++)
        {
            int root = _field.Exp(_field.GeneratorBase + i);
            int[] next = new int[g.Length + 1];
            for (int j = 0; j < g.Length; j++)
            {
                next[j] ^= g[j];
                next[j + 1] ^= _field.Multiply(g[j], root);
            }

            g = next;
        }

        return g;
    }
}
