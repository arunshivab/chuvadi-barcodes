// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;
using Xunit;

namespace Chuvadi.Barcodes.Tests;

public sealed class LuminanceImageTests
{
    [Fact]
    public void Indexer_ReadsRowMajor()
    {
        LuminanceImage image = new(3, 2, [0, 1, 2, 3, 4, 5]);

        Assert.Equal(5, image[2, 1]);
        Assert.Equal(1, image[1, 0]);
    }

    [Fact]
    public void WrongBufferLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => new LuminanceImage(3, 2, new byte[5]));
    }
}
