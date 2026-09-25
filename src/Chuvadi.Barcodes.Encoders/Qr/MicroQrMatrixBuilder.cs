// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — Micro QR module placement and masking

using System;

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>
/// Places function patterns and data for a Micro QR symbol (M1–M4), applies one of the four
/// Micro QR masks and writes the format information.
/// </summary>
internal sealed class MicroQrMatrixBuilder
{
    private readonly int _size;
    private readonly bool[,] _modules;
    private readonly bool[,] _isFunction;

    private MicroQrMatrixBuilder(int version)
    {
        _size = (version * 2) + 9;
        _modules = new bool[_size, _size];
        _isFunction = new bool[_size, _size];
    }

    /// <summary>
    /// Builds the symbol from the data bit stream (exactly the data capacity, including a
    /// final 4-bit codeword where applicable) followed by the ECC codewords.
    /// </summary>
    public static BitMatrix Build(int version, int symbolNumber, bool[] bits, int? mask, out int chosenMask)
    {
        MicroQrMatrixBuilder b = new(version);
        b.DrawFunctionPatterns();
        b.DrawData(bits);

        if (mask is int fixedMask)
        {
            chosenMask = fixedMask;
        }
        else
        {
            chosenMask = 0;
            int bestScore = -1;
            for (int m = 0; m < 4; m++)
            {
                b.ApplyMask(m);
                int score = b.Evaluate();
                if (score > bestScore)
                {
                    bestScore = score;
                    chosenMask = m;
                }

                b.ApplyMask(m);
            }
        }

        b.ApplyMask(chosenMask);
        b.DrawFormatBits(symbolNumber, chosenMask);
        return b.ToBitMatrix();
    }

    /// <summary>
    /// Micro QR mask conditions; they are QR masks 1, 4, 6 and 7 (x = column, y = row).
    /// </summary>
    public static bool MaskCondition(int mask, int x, int y) => mask switch
    {
        0 => y % 2 == 0,
        1 => ((y / 2) + (x / 3)) % 2 == 0,
        2 => (((x * y) % 2) + ((x * y) % 3)) % 2 == 0,
        _ => (((x + y) % 2) + ((x * y) % 3)) % 2 == 0,
    };

    /// <summary>15-bit format information (symbol number and mask, BCH(15,5), XOR 0x4445).</summary>
    public static int FormatBits(int symbolNumber, int mask)
    {
        int data = (symbolNumber << 2) | mask;
        return ((data << 10) | QrMatrixBuilder.Bch(data, 0x537, 10)) ^ 0x4445;
    }

    private void SetFunction(int x, int y, bool dark)
    {
        _modules[y, x] = dark;
        _isFunction[y, x] = true;
    }

    private void DrawFunctionPatterns()
    {
        // Finder pattern with separator (right and bottom only).
        for (int y = 0; y <= 7; y++)
        {
            for (int x = 0; x <= 7; x++)
            {
                int dist = Math.Max(Math.Abs(x - 3), Math.Abs(y - 3));
                SetFunction(x, y, dist != 2 && dist != 4);
            }
        }

        // Timing patterns along the top row and left column.
        for (int i = 8; i < _size; i++)
        {
            SetFunction(i, 0, i % 2 == 0);
            SetFunction(0, i, i % 2 == 0);
        }

        // Reserve format information area.
        for (int i = 1; i <= 8; i++)
        {
            SetFunction(i, 8, false);
            SetFunction(8, i, false);
        }
    }

    private void DrawData(bool[] bits)
    {
        int i = 0;
        bool upward = true;
        for (int right = _size - 1; right >= 1; right -= 2)
        {
            for (int vert = 0; vert < _size; vert++)
            {
                int y = upward ? _size - 1 - vert : vert;
                for (int j = 0; j < 2; j++)
                {
                    int x = right - j;
                    if (!_isFunction[y, x] && i < bits.Length)
                    {
                        _modules[y, x] = bits[i];
                        i++;
                    }
                }
            }

            upward = !upward;
        }
    }

    private void ApplyMask(int mask)
    {
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                if (!_isFunction[y, x] && MaskCondition(mask, x, y))
                {
                    _modules[y, x] = !_modules[y, x];
                }
            }
        }
    }

    // Micro QR mask evaluation: dark modules on the right and bottom edges (excluding timing).
    private int Evaluate()
    {
        int sum1 = 0;
        int sum2 = 0;
        for (int i = 1; i < _size; i++)
        {
            if (_modules[i, _size - 1])
            {
                sum1++;
            }

            if (_modules[_size - 1, i])
            {
                sum2++;
            }
        }

        return sum1 <= sum2 ? (sum1 * 16) + sum2 : (sum2 * 16) + sum1;
    }

    private void DrawFormatBits(int symbolNumber, int mask)
    {
        int bits = FormatBits(symbolNumber, mask);

        // Bits 14..7 along row 8 (columns 1..8); bits 6..0 up column 8 (rows 7..1).
        for (int i = 0; i < 8; i++)
        {
            SetFunction(i + 1, 8, ((bits >> (14 - i)) & 1) != 0);
        }

        for (int i = 0; i < 7; i++)
        {
            SetFunction(8, 7 - i, ((bits >> (6 - i)) & 1) != 0);
        }
    }

    private BitMatrix ToBitMatrix()
    {
        BitMatrix matrix = new(_size, _size);
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                if (_modules[y, x])
                {
                    matrix[x, y] = true;
                }
            }
        }

        return matrix;
    }
}
