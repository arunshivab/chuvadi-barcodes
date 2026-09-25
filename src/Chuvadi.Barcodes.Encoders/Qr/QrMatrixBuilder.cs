// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR Code Model 2 module placement, masking and penalty scoring

using System;

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>
/// Places function patterns and codewords for a QR Code Model 2 symbol, applies a mask and
/// writes format and version information.
/// </summary>
internal sealed class QrMatrixBuilder
{
    private const int PenaltyN1 = 3;
    private const int PenaltyN2 = 3;
    private const int PenaltyN3 = 40;
    private const int PenaltyN4 = 10;

    private readonly int _version;
    private readonly int _size;
    private readonly bool[,] _modules;
    private readonly bool[,] _isFunction;

    private QrMatrixBuilder(int version)
    {
        _version = version;
        _size = (version * 4) + 17;
        _modules = new bool[_size, _size];
        _isFunction = new bool[_size, _size];
    }

    /// <summary>Builds the symbol. <paramref name="mask"/> null selects the lowest-penalty mask.</summary>
    public static BitMatrix Build(int version, QrErrorCorrectionLevel level, byte[] codewords, int? mask, out int chosenMask)
    {
        QrMatrixBuilder b = new(version);
        b.DrawFunctionPatterns();
        b.DrawCodewords(codewords);

        if (mask is int fixedMask)
        {
            chosenMask = fixedMask;
        }
        else
        {
            chosenMask = 0;
            long minPenalty = long.MaxValue;
            for (int m = 0; m < 8; m++)
            {
                b.ApplyMask(m);
                b.DrawFormatBits(level, m);
                long penalty = b.PenaltyScore();
                if (penalty < minPenalty)
                {
                    chosenMask = m;
                    minPenalty = penalty;
                }

                b.ApplyMask(m); // XOR again to undo
            }
        }

        b.ApplyMask(chosenMask);
        b.DrawFormatBits(level, chosenMask);
        return b.ToBitMatrix();
    }

    /// <summary>True when mask pattern <paramref name="mask"/> inverts module (x, y).</summary>
    public static bool MaskCondition(int mask, int x, int y) => mask switch
    {
        0 => (x + y) % 2 == 0,
        1 => y % 2 == 0,
        2 => x % 3 == 0,
        3 => (x + y) % 3 == 0,
        4 => ((x / 3) + (y / 2)) % 2 == 0,
        5 => ((x * y) % 2) + ((x * y) % 3) == 0,
        6 => (((x * y) % 2) + ((x * y) % 3)) % 2 == 0,
        _ => (((x + y) % 2) + ((x * y) % 3)) % 2 == 0,
    };

    /// <summary>15-bit format information (BCH(15,5), XOR 0x5412).</summary>
    public static int FormatBits(QrErrorCorrectionLevel level, int mask)
    {
        int levelBits = level switch
        {
            QrErrorCorrectionLevel.L => 1,
            QrErrorCorrectionLevel.M => 0,
            QrErrorCorrectionLevel.Q => 3,
            _ => 2,
        };
        int data = (levelBits << 3) | mask;
        return ((data << 10) | Bch(data, 0x537, 10)) ^ 0x5412;
    }

    /// <summary>BCH remainder of <paramref name="data"/> by <paramref name="generator"/>.</summary>
    public static int Bch(int data, int generator, int remainderBits)
    {
        int rem = data;
        for (int i = 0; i < remainderBits; i++)
        {
            rem = (rem << 1) ^ ((rem >> (remainderBits - 1)) * generator);
        }

        return rem & ((1 << remainderBits) - 1);
    }

    private void SetFunction(int x, int y, bool dark)
    {
        _modules[y, x] = dark;
        _isFunction[y, x] = true;
    }

    private void DrawFunctionPatterns()
    {
        for (int i = 0; i < _size; i++)
        {
            SetFunction(6, i, i % 2 == 0);
            SetFunction(i, 6, i % 2 == 0);
        }

        DrawFinder(3, 3);
        DrawFinder(_size - 4, 3);
        DrawFinder(3, _size - 4);

        int[] positions = QrTables.AlignmentPositions(_version);
        int count = positions.Length;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                bool corner = (i == 0 && j == 0) || (i == 0 && j == count - 1) || (i == count - 1 && j == 0);
                if (!corner)
                {
                    DrawAlignment(positions[i], positions[j]);
                }
            }
        }

        DrawFormatBits(QrErrorCorrectionLevel.L, 0); // reserve; rewritten after masking
        DrawVersionBits();
    }

    private void DrawFinder(int cx, int cy)
    {
        for (int dy = -4; dy <= 4; dy++)
        {
            for (int dx = -4; dx <= 4; dx++)
            {
                int x = cx + dx;
                int y = cy + dy;
                if (x < 0 || x >= _size || y < 0 || y >= _size)
                {
                    continue;
                }

                int dist = Math.Max(Math.Abs(dx), Math.Abs(dy));
                SetFunction(x, y, dist != 2 && dist != 4);
            }
        }
    }

    private void DrawAlignment(int cx, int cy)
    {
        for (int dy = -2; dy <= 2; dy++)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                SetFunction(cx + dx, cy + dy, Math.Max(Math.Abs(dx), Math.Abs(dy)) != 1);
            }
        }
    }

    private void DrawFormatBits(QrErrorCorrectionLevel level, int mask)
    {
        int bits = FormatBits(level, mask);

        for (int i = 0; i <= 5; i++)
        {
            SetFunction(8, i, Bit(bits, i));
        }

        SetFunction(8, 7, Bit(bits, 6));
        SetFunction(8, 8, Bit(bits, 7));
        SetFunction(7, 8, Bit(bits, 8));
        for (int i = 9; i < 15; i++)
        {
            SetFunction(14 - i, 8, Bit(bits, i));
        }

        for (int i = 0; i < 8; i++)
        {
            SetFunction(_size - 1 - i, 8, Bit(bits, i));
        }

        for (int i = 8; i < 15; i++)
        {
            SetFunction(8, _size - 15 + i, Bit(bits, i));
        }

        SetFunction(8, _size - 8, true); // dark module
    }

    private void DrawVersionBits()
    {
        if (_version < 7)
        {
            return;
        }

        int bits = (_version << 12) | Bch(_version, 0x1F25, 12);
        for (int i = 0; i < 18; i++)
        {
            bool bit = Bit(bits, i);
            int a = _size - 11 + (i % 3);
            int b = i / 3;
            SetFunction(a, b, bit);
            SetFunction(b, a, bit);
        }
    }

    private void DrawCodewords(byte[] codewords)
    {
        int i = 0;
        int totalBits = codewords.Length * 8;
        for (int right = _size - 1; right >= 1; right -= 2)
        {
            if (right == 6)
            {
                right = 5;
            }

            for (int vert = 0; vert < _size; vert++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int x = right - j;
                    bool upward = ((right + 1) & 2) == 0;
                    int y = upward ? _size - 1 - vert : vert;
                    if (!_isFunction[y, x] && i < totalBits)
                    {
                        _modules[y, x] = ((codewords[i >> 3] >> (7 - (i & 7))) & 1) != 0;
                        i++;
                    }
                }
            }
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

    private long PenaltyScore()
    {
        long result = 0;

        for (int y = 0; y < _size; y++)
        {
            result += LinePenalty(y, horizontal: true);
        }

        for (int x = 0; x < _size; x++)
        {
            result += LinePenalty(x, horizontal: false);
        }

        for (int y = 0; y < _size - 1; y++)
        {
            for (int x = 0; x < _size - 1; x++)
            {
                bool c = _modules[y, x];
                if (c == _modules[y, x + 1] && c == _modules[y + 1, x] && c == _modules[y + 1, x + 1])
                {
                    result += PenaltyN2;
                }
            }
        }

        int dark = 0;
        foreach (bool m in _modules)
        {
            if (m)
            {
                dark++;
            }
        }

        int total = _size * _size;
        int k = ((Math.Abs((dark * 20) - (total * 10)) + total - 1) / total) - 1;
        result += k * PenaltyN4;
        return result;
    }

    // Rules 1 and 3 for one row or column.
    // Rule 1: each run of five or more same-colour modules scores 3 + (length - 5).
    // Rule 3: each dark:light:dark:light:dark 1:1:3:1:1 pattern (1011101) that is preceded
    // or followed by four light modules scores 40 once. Modules outside the symbol count
    // as light (the quiet zone). This reading reproduces Zint's mask choices exactly.
    private long LinePenalty(int line, bool horizontal)
    {
        long result = 0;
        int runLength = 1;
        for (int i = 1; i <= _size; i++)
        {
            if (i < _size && Module(line, i, horizontal) == Module(line, i - 1, horizontal))
            {
                runLength++;
                continue;
            }

            if (runLength >= 5)
            {
                result += PenaltyN1 + (runLength - 5);
            }

            runLength = 1;
        }

        for (int i = 0; i + 7 <= _size; i++)
        {
            if (Module(line, i, horizontal)
                && !Module(line, i + 1, horizontal)
                && Module(line, i + 2, horizontal)
                && Module(line, i + 3, horizontal)
                && Module(line, i + 4, horizontal)
                && !Module(line, i + 5, horizontal)
                && Module(line, i + 6, horizontal)
                && (IsLightRange(line, i - 4, i, horizontal) || IsLightRange(line, i + 7, i + 11, horizontal)))
            {
                result += PenaltyN3;
            }
        }

        return result;
    }

    private bool Module(int line, int i, bool horizontal) => horizontal ? _modules[line, i] : _modules[i, line];

    private bool IsLightRange(int line, int from, int to, bool horizontal)
    {
        for (int i = from; i < to; i++)
        {
            if (i >= 0 && i < _size && Module(line, i, horizontal))
            {
                return false;
            }
        }

        return true;
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

    private static bool Bit(int value, int index) => ((value >> index) & 1) != 0;
}
