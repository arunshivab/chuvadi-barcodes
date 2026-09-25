// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — CRC-32 (ISO-HDLC / PNG) without external packages

using System;

namespace Chuvadi.Barcodes.Imaging;

/// <summary>CRC-32 with polynomial 0xEDB88320 (reflected), as used by PNG and zlib.</summary>
internal static class Crc32
{
    private static readonly uint[] Table = BuildTable();

    /// <summary>Continues a CRC over <paramref name="data"/>. Start with 0xFFFFFFFF and XOR the result with 0xFFFFFFFF.</summary>
    public static uint Update(uint crc, ReadOnlySpan<byte> data)
    {
        foreach (byte b in data)
        {
            crc = Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
        }

        return crc;
    }

    /// <summary>CRC-32 of <paramref name="data"/>.</summary>
    public static uint Compute(ReadOnlySpan<byte> data) => Update(0xFFFFFFFFu, data) ^ 0xFFFFFFFFu;

    private static uint[] BuildTable()
    {
        uint[] table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            }

            table[n] = c;
        }

        return table;
    }
}
