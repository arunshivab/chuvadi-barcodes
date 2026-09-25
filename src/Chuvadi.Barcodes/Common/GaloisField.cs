// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;

namespace Chuvadi.Barcodes.Common;

/// <summary>
/// Arithmetic in a binary Galois field GF(2^m), defined by a primitive polynomial.
/// Addition and subtraction are XOR; multiplication and division use log/antilog tables.
/// </summary>
internal sealed class GaloisField
{
    private readonly int[] _exp;
    private readonly int[] _log;

    /// <summary>GF(256) with polynomial x^8 + x^4 + x^3 + x^2 + 1 (0x11D) — QR Code, Micro QR, rMQR.</summary>
    public static readonly GaloisField QrCode256 = new(0x11D, 256, 0);

    /// <summary>Creates a field.</summary>
    /// <param name="primitive">Primitive polynomial including the x^m term, e.g. 0x11D.</param>
    /// <param name="size">Field size 2^m.</param>
    /// <param name="generatorBase">Exponent of the first root of the RS generator polynomial (0 or 1).</param>
    public GaloisField(int primitive, int size, int generatorBase)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 4);
        Primitive = primitive;
        Size = size;
        GeneratorBase = generatorBase;
        _exp = new int[size * 2];
        _log = new int[size];
        int x = 1;
        for (int i = 0; i < size - 1; i++)
        {
            _exp[i] = x;
            _log[x] = i;
            x <<= 1;
            if (x >= size)
            {
                x ^= primitive;
            }
        }

        for (int i = size - 1; i < _exp.Length; i++)
        {
            _exp[i] = _exp[i - (size - 1)];
        }
    }

    /// <summary>The primitive polynomial.</summary>
    public int Primitive { get; }

    /// <summary>Number of elements, 2^m.</summary>
    public int Size { get; }

    /// <summary>Exponent of the first generator root.</summary>
    public int GeneratorBase { get; }

    /// <summary>α^power.</summary>
    public int Exp(int power)
    {
        int n = Size - 1;
        power %= n;
        if (power < 0)
        {
            power += n;
        }

        return _exp[power];
    }

    /// <summary>log_α(value); value must be non-zero.</summary>
    public int Log(int value)
    {
        if (value == 0)
        {
            throw new ArgumentException("log(0) is undefined.", nameof(value));
        }

        return _log[value];
    }

    /// <summary>Field multiplication.</summary>
    public int Multiply(int a, int b)
    {
        if (a == 0 || b == 0)
        {
            return 0;
        }

        return _exp[_log[a] + _log[b]];
    }

    /// <summary>Multiplicative inverse; value must be non-zero.</summary>
    public int Inverse(int value)
    {
        if (value == 0)
        {
            throw new DivideByZeroException();
        }

        return _exp[Size - 1 - _log[value]];
    }
}
