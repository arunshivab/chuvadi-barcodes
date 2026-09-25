// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — PNG writer (8-bit greyscale). Readers for all formats arrive in M3.

using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Chuvadi.Barcodes.Imaging;

/// <summary>Writes PNG files. Uses only the .NET base library (zlib via <see cref="ZLibStream"/>).</summary>
public static class PngWriter
{
    private static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>Encodes an 8-bit greyscale image as PNG.</summary>
    /// <param name="width">Width in pixels; must be positive.</param>
    /// <param name="height">Height in pixels; must be positive.</param>
    /// <param name="pixels">Row-major pixels, exactly <paramref name="width"/> × <paramref name="height"/> bytes (0 = black).</param>
    /// <param name="dpi">Resolution to record in a pHYs chunk, or 0 to omit it.</param>
    /// <returns>The PNG file bytes.</returns>
    public static byte[] WriteGray8(int width, int height, ReadOnlySpan<byte> pixels, int dpi = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(dpi);
        if (pixels.Length != (long)width * height)
        {
            throw new ArgumentException($"Expected {(long)width * height} bytes, got {pixels.Length}.", nameof(pixels));
        }

        using MemoryStream output = new();
        output.Write(Signature);

        byte[] header = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(0), width);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), height);
        header[8] = 8; // bit depth
        header[9] = 0; // colour type: greyscale
        header[10] = 0; // compression
        header[11] = 0; // filter
        header[12] = 0; // interlace
        WriteChunk(output, "IHDR", header);

        if (dpi > 0)
        {
            byte[] phys = new byte[9];
            uint pixelsPerMetre = (uint)Math.Round(dpi / 0.0254);
            BinaryPrimitives.WriteUInt32BigEndian(phys.AsSpan(0), pixelsPerMetre);
            BinaryPrimitives.WriteUInt32BigEndian(phys.AsSpan(4), pixelsPerMetre);
            phys[8] = 1; // unit: metre
            WriteChunk(output, "pHYs", phys);
        }

        using (MemoryStream compressed = new())
        {
            using (ZLibStream zlib = new(compressed, CompressionLevel.SmallestSize, leaveOpen: true))
            {
                byte[] row = new byte[width + 1];
                for (int y = 0; y < height; y++)
                {
                    row[0] = 0; // filter type None
                    pixels.Slice(y * width, width).CopyTo(row.AsSpan(1));
                    zlib.Write(row);
                }
            }

            WriteChunk(output, "IDAT", compressed.ToArray());
        }

        WriteChunk(output, "IEND", []);
        return output.ToArray();
    }

    private static void WriteChunk(Stream output, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        output.Write(length);

        byte[] typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        uint crc = Crc32.Update(0xFFFFFFFFu, typeBytes);
        crc = Crc32.Update(crc, data) ^ 0xFFFFFFFFu;
        Span<byte> crcBytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crcBytes, crc);
        output.Write(crcBytes);
    }
}
