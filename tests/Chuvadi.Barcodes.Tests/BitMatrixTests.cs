// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;
using Xunit;

namespace Chuvadi.Barcodes.Tests;

public sealed class BitMatrixTests
{
    [Fact]
    public void NewMatrix_IsAllLight()
    {
        BitMatrix m = new(40, 3);

        Assert.Equal(0, m.CountDark());
        Assert.False(m[39, 2]);
    }

    [Fact]
    public void SetGetFlip_WorkAcrossWordBoundaries()
    {
        BitMatrix m = new(70, 2);

        m[0, 0] = true;
        m[31, 0] = true;
        m[32, 1] = true;
        m[69, 1] = true;
        m.Flip(31, 0);

        Assert.True(m[0, 0]);
        Assert.False(m[31, 0]);
        Assert.True(m[32, 1]);
        Assert.True(m[69, 1]);
        Assert.Equal(3, m.CountDark());
    }

    [Fact]
    public void ParseAndToString_RoundTrip()
    {
        const string Text = "101\n010\n111\n000\n";

        BitMatrix m = BitMatrix.Parse(Text);

        Assert.Equal(3, m.Width);
        Assert.Equal(4, m.Height);
        Assert.Equal(Text, m.ToString());
    }

    [Fact]
    public void Parse_RejectsRaggedRowsAndBadCharacters()
    {
        Assert.Throws<FormatException>(() => BitMatrix.Parse("10\n1"));
        Assert.Throws<FormatException>(() => BitMatrix.Parse("1x"));
        Assert.Throws<FormatException>(() => BitMatrix.Parse("\n\n"));
    }

    [Fact]
    public void Equality_ComparesDimensionsAndModules()
    {
        BitMatrix a = BitMatrix.Parse("10\n01");
        BitMatrix b = BitMatrix.Parse("10\n01");
        BitMatrix c = BitMatrix.Parse("10\n00");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.NotEqual(a, c);
        Assert.Equal(a, a.Clone());
    }

    [Fact]
    public void OutOfRange_Throws()
    {
        BitMatrix m = new(5, 5);

        Assert.Throws<ArgumentOutOfRangeException>(() => m[5, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => m[0, -1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => new BitMatrix(0, 1));
    }
}
