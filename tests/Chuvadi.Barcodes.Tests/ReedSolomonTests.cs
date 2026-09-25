// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — Galois field and Reed–Solomon encoder

using Chuvadi.Barcodes.Common;
using Xunit;

namespace Chuvadi.Barcodes.Tests;

public sealed class ReedSolomonTests
{
    // The widely published worked example: "HELLO WORLD", QR version 1-M.
    private static readonly byte[] HelloWorldData = [32, 91, 11, 120, 209, 114, 220, 77, 67, 64, 236, 17, 236, 17, 236, 17];
    private static readonly byte[] HelloWorldEcc = [196, 35, 39, 119, 235, 215, 231, 226, 93, 23];

    [Fact]
    public void QrHelloWorldVersion1M_ProducesPublishedEcc()
    {
        ReedSolomonEncoder rs = new(GaloisField.QrCode256);

        byte[] ecc = rs.Encode(HelloWorldData, 10);

        Assert.Equal(HelloWorldEcc, ecc);
    }

    [Fact]
    public void Gf256_EveryNonZeroElementHasAnInverse()
    {
        GaloisField f = GaloisField.QrCode256;

        for (int a = 1; a < 256; a++)
        {
            Assert.Equal(1, f.Multiply(a, f.Inverse(a)));
        }
    }

    [Fact]
    public void Gf256_ExpAndLogAreInverse()
    {
        GaloisField f = GaloisField.QrCode256;

        for (int a = 1; a < 256; a++)
        {
            Assert.Equal(a, f.Exp(f.Log(a)));
        }
    }

    [Fact]
    public void Codeword_IsDivisibleByGenerator()
    {
        // A valid RS codeword evaluates to zero at every generator root.
        GaloisField f = GaloisField.QrCode256;
        ReedSolomonEncoder rs = new(f);
        byte[] ecc = rs.Encode(HelloWorldData, 10);
        byte[] codeword = [.. HelloWorldData, .. ecc];

        for (int root = 0; root < 10; root++)
        {
            int alpha = f.Exp(root);
            int value = 0;
            foreach (byte c in codeword)
            {
                value = f.Multiply(value, alpha) ^ c;
            }

            Assert.Equal(0, value);
        }
    }

    [Fact]
    public void BitBuffer_PacksMsbFirst()
    {
        BitBuffer b = new();
        b.Append(0b0100, 4);
        b.Append(0b1, 1);
        b.Append(0b101, 3);
        b.Append(0b1, 1);

        Assert.Equal(9, b.Length);
        Assert.Equal(new byte[] { 0b0100_1101, 0b1000_0000 }, b.ToBytes());
    }
}
