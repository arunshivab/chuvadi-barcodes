// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — PNG writer

using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;
using Xunit;

namespace Chuvadi.Barcodes.Imaging.Tests;

public sealed class PngWriterTests
{
    private static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    [Fact]
    public void Crc32_MatchesStandardCheckValue()
    {
        Assert.Equal(0xCBF43926u, Crc32.Compute(Encoding.ASCII.GetBytes("123456789")));
    }

    [Fact]
    public void Gray8_ProducesValidChunksAndPixels()
    {
        byte[] pixels = [0, 255, 128, 64, 32, 16];

        byte[] png = PngWriter.WriteGray8(3, 2, pixels, dpi: 300);

        Assert.Equal(Signature, png[..8]);
        int offset = 8;
        MemoryStream idat = new();
        bool sawIend = false;
        while (offset < png.Length)
        {
            int length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset));
            string type = Encoding.ASCII.GetString(png, offset + 4, 4);
            ReadOnlySpan<byte> typeAndData = png.AsSpan(offset + 4, 4 + length);
            uint crc = BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset + 8 + length));
            Assert.Equal(Crc32.Compute(typeAndData), crc);

            if (type == "IHDR")
            {
                Assert.Equal(3, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset + 8)));
                Assert.Equal(2, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset + 12)));
                Assert.Equal(8, png[offset + 16]);
                Assert.Equal(0, png[offset + 17]);
            }
            else if (type == "pHYs")
            {
                Assert.Equal(11811u, BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset + 8)));
            }
            else if (type == "IDAT")
            {
                idat.Write(png, offset + 8, length);
            }
            else if (type == "IEND")
            {
                sawIend = true;
            }

            offset += 12 + length;
        }

        Assert.True(sawIend);
        idat.Position = 0;
        using ZLibStream z = new(idat, CompressionMode.Decompress);
        byte[] raw = new byte[8];
        z.ReadExactly(raw);
        Assert.Equal(new byte[] { 0, 0, 255, 128, 0, 64, 32, 16 }, raw);
    }

    [Fact]
    public void WrongPixelCount_Throws()
    {
        Assert.Throws<ArgumentException>(() => PngWriter.WriteGray8(2, 2, new byte[3]));
    }
}
