// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;
using System.Text;

namespace Chuvadi.Barcodes;

/// <summary>
/// A two-dimensional grid of modules, each either dark (<see langword="true"/>) or light
/// (<see langword="false"/>). Coordinates are (x, y) with (0, 0) at the top-left corner;
/// x grows to the right and y grows downwards. The quiet zone is not included.
/// </summary>
public sealed class BitMatrix : IEquatable<BitMatrix>
{
    private readonly uint[] _bits;
    private readonly int _rowWords;

    /// <summary>Creates an all-light matrix.</summary>
    /// <param name="width">Number of columns; must be positive.</param>
    /// <param name="height">Number of rows; must be positive.</param>
    public BitMatrix(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        Width = width;
        Height = height;
        _rowWords = (width + 31) / 32;
        _bits = new uint[_rowWords * height];
    }

    /// <summary>Number of columns.</summary>
    public int Width { get; }

    /// <summary>Number of rows.</summary>
    public int Height { get; }

    /// <summary>Gets or sets the module at column <paramref name="x"/>, row <paramref name="y"/>.</summary>
    /// <param name="x">Column, from 0 to <see cref="Width"/> - 1.</param>
    /// <param name="y">Row, from 0 to <see cref="Height"/> - 1.</param>
    /// <returns><see langword="true"/> when the module is dark.</returns>
    public bool this[int x, int y]
    {
        get
        {
            int index = Index(x, y);
            return (_bits[index] & (1u << (x & 31))) != 0;
        }

        set
        {
            int index = Index(x, y);
            if (value)
            {
                _bits[index] |= 1u << (x & 31);
            }
            else
            {
                _bits[index] &= ~(1u << (x & 31));
            }
        }
    }

    /// <summary>Inverts the module at the given position.</summary>
    /// <param name="x">Column.</param>
    /// <param name="y">Row.</param>
    public void Flip(int x, int y)
    {
        int index = Index(x, y);
        _bits[index] ^= 1u << (x & 31);
    }

    /// <summary>Counts the dark modules in the whole matrix.</summary>
    /// <returns>The number of dark modules.</returns>
    public int CountDark()
    {
        int count = 0;
        foreach (uint word in _bits)
        {
            count += System.Numerics.BitOperations.PopCount(word);
        }

        return count;
    }

    /// <summary>Creates an independent copy of this matrix.</summary>
    /// <returns>The copy.</returns>
    public BitMatrix Clone()
    {
        BitMatrix copy = new(Width, Height);
        Array.Copy(_bits, copy._bits, _bits.Length);
        return copy;
    }

    /// <summary>
    /// Parses the text form produced by <see cref="ToString"/>: one line per row, each
    /// character <c>'1'</c> (dark) or <c>'0'</c> (light). Blank lines are ignored.
    /// </summary>
    /// <param name="text">The rows.</param>
    /// <returns>The matrix.</returns>
    /// <exception cref="FormatException">The rows are empty, ragged, or contain other characters.</exception>
    public static BitMatrix Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        string[] rows = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rows.Length == 0)
        {
            throw new FormatException("No rows found.");
        }

        BitMatrix matrix = new(rows[0].Length, rows.Length);
        for (int y = 0; y < rows.Length; y++)
        {
            string row = rows[y];
            if (row.Length != matrix.Width)
            {
                throw new FormatException($"Row {y} has {row.Length} modules; expected {matrix.Width}.");
            }

            for (int x = 0; x < row.Length; x++)
            {
                char c = row[x];
                if (c == '1')
                {
                    matrix[x, y] = true;
                }
                else if (c != '0')
                {
                    throw new FormatException($"Invalid character '{c}' at row {y}, column {x}.");
                }
            }
        }

        return matrix;
    }

    /// <summary>Returns the rows as lines of <c>'1'</c> (dark) and <c>'0'</c> (light), separated by <c>'\n'</c>.</summary>
    /// <returns>The text form.</returns>
    public override string ToString()
    {
        StringBuilder sb = new((Width + 1) * Height);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(this[x, y] ? '1' : '0');
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }

    /// <summary>Compares dimensions and every module.</summary>
    /// <param name="other">The other matrix.</param>
    /// <returns><see langword="true"/> when both matrices are identical.</returns>
    public bool Equals(BitMatrix? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Width == other.Width && Height == other.Height && _bits.AsSpan().SequenceEqual(other._bits);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as BitMatrix);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(Width);
        hash.Add(Height);
        foreach (uint word in _bits)
        {
            hash.Add(word);
        }

        return hash.ToHashCode();
    }

    private int Index(int x, int y)
    {
        if ((uint)x >= (uint)Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if ((uint)y >= (uint)Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return (y * _rowWords) + (x >> 5);
    }
}
