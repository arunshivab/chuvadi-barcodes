// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR tables (verified module-for-module against Zint golden fixtures)

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>Capacity and error-correction block tables for QR Code Model 2 and Micro QR.</summary>
internal static class QrTables
{
    // Index [level][version]; level order L, M, Q, H; version 0 unused.
    private static readonly int[][] EccCodewordsPerBlock =
    [
        [-1, 7, 10, 15, 20, 26, 18, 20, 24, 30, 18, 20, 24, 26, 30, 22, 24, 28, 30, 28, 28, 28, 28, 30, 30, 26, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
        [-1, 10, 16, 26, 18, 24, 16, 18, 22, 22, 26, 30, 22, 22, 24, 24, 28, 28, 26, 26, 26, 26, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28],
        [-1, 13, 22, 18, 26, 18, 24, 18, 22, 20, 24, 28, 26, 24, 20, 30, 24, 28, 28, 26, 30, 28, 30, 30, 30, 30, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
        [-1, 17, 28, 22, 16, 22, 28, 26, 26, 24, 28, 24, 28, 22, 24, 24, 30, 28, 28, 26, 28, 30, 24, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
    ];

    private static readonly int[][] ErrorCorrectionBlocks =
    [
        [-1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 7, 8, 8, 9, 9, 10, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 24, 25],
        [-1, 1, 1, 1, 2, 2, 4, 4, 4, 5, 5, 5, 8, 9, 9, 10, 10, 11, 13, 14, 16, 17, 17, 18, 20, 21, 23, 25, 26, 28, 29, 31, 33, 35, 37, 38, 40, 43, 45, 47, 49],
        [-1, 1, 1, 2, 2, 4, 4, 6, 6, 8, 8, 8, 10, 12, 16, 12, 17, 16, 18, 21, 20, 23, 23, 25, 27, 29, 34, 34, 35, 38, 40, 43, 45, 48, 51, 53, 56, 59, 62, 65, 68],
        [-1, 1, 1, 2, 4, 4, 4, 5, 6, 8, 8, 11, 11, 16, 16, 18, 16, 19, 21, 25, 25, 25, 34, 30, 32, 35, 37, 40, 42, 45, 48, 51, 54, 57, 60, 63, 66, 70, 74, 77, 81],
    ];

    /// <summary>Error-correction codewords per block.</summary>
    public static int EccPerBlock(int version, QrErrorCorrectionLevel level) => EccCodewordsPerBlock[(int)level][version];

    /// <summary>Number of error-correction blocks.</summary>
    public static int BlockCount(int version, QrErrorCorrectionLevel level) => ErrorCorrectionBlocks[(int)level][version];

    /// <summary>Modules available for data and ECC codewords (including remainder bits).</summary>
    public static int RawDataModules(int version)
    {
        int result = (((16 * version) + 128) * version) + 64;
        if (version >= 2)
        {
            int alignCount = (version / 7) + 2;
            result -= (((25 * alignCount) - 10) * alignCount) - 55;
            if (version >= 7)
            {
                result -= 36;
            }
        }

        return result;
    }

    /// <summary>Total codewords (data + ECC).</summary>
    public static int TotalCodewords(int version) => RawDataModules(version) / 8;

    /// <summary>Data codewords for the given version and level.</summary>
    public static int DataCodewords(int version, QrErrorCorrectionLevel level) =>
        TotalCodewords(version) - (EccPerBlock(version, level) * BlockCount(version, level));

    /// <summary>Centre coordinates of the alignment patterns (same for rows and columns).</summary>
    public static int[] AlignmentPositions(int version)
    {
        if (version == 1)
        {
            return [];
        }

        int count = (version / 7) + 2;
        int size = (version * 4) + 17;
        int step = (((version * 8) + (count * 3) + 5) / ((count * 4) - 4)) * 2;
        int[] result = new int[count];
        result[0] = 6;
        for (int i = count - 1, pos = size - 7; i >= 1; i--, pos -= step)
        {
            result[i] = pos;
        }

        return result;
    }

    /// <summary>Character-count indicator length for QR Code Model 2.</summary>
    public static int CharCountBits(QrMode mode, int version)
    {
        int group = version <= 9 ? 0 : version <= 26 ? 1 : 2;
        return mode switch
        {
            QrMode.Numeric => group switch { 0 => 10, 1 => 12, _ => 14 },
            QrMode.Alphanumeric => group switch { 0 => 9, 1 => 11, _ => 13 },
            QrMode.Byte => group == 0 ? 8 : 16,
            _ => group switch { 0 => 8, 1 => 10, _ => 12 },
        };
    }

    // ── Micro QR ──────────────────────────────────────────────────────────

    /// <summary>
    /// Micro QR symbol number (0–7) for version/level, or -1 when the combination does not exist.
    /// </summary>
    public static int MicroSymbolNumber(int version, QrErrorCorrectionLevel level) => (version, level) switch
    {
        (1, QrErrorCorrectionLevel.L) => 0,
        (2, QrErrorCorrectionLevel.L) => 1,
        (2, QrErrorCorrectionLevel.M) => 2,
        (3, QrErrorCorrectionLevel.L) => 3,
        (3, QrErrorCorrectionLevel.M) => 4,
        (4, QrErrorCorrectionLevel.L) => 5,
        (4, QrErrorCorrectionLevel.M) => 6,
        (4, QrErrorCorrectionLevel.Q) => 7,
        _ => -1,
    };

    private static readonly int[] MicroDataBits = [20, 40, 32, 84, 68, 128, 112, 80];
    private static readonly int[] MicroEccCodewords = [2, 5, 6, 6, 8, 8, 10, 14];
    private static readonly int[] MicroTotalCodewords = [5, 10, 17, 24];

    /// <summary>Data capacity in bits for a Micro QR symbol number.</summary>
    public static int MicroDataBitCount(int symbolNumber) => MicroDataBits[symbolNumber];

    /// <summary>ECC codewords for a Micro QR symbol number.</summary>
    public static int MicroEccCount(int symbolNumber) => MicroEccCodewords[symbolNumber];

    /// <summary>Total codewords (data + ECC; a 4-bit final data codeword counts as one) for a Micro QR version.</summary>
    public static int MicroTotalCount(int version) => MicroTotalCodewords[version - 1];

    /// <summary>Character-count indicator length for Micro QR, or 0 when the mode is unavailable.</summary>
    public static int MicroCharCountBits(QrMode mode, int version) => mode switch
    {
        QrMode.Numeric => version + 2,
        QrMode.Alphanumeric => version >= 2 ? version + 1 : 0,
        QrMode.Byte => version >= 3 ? version + 1 : 0,
        _ => version >= 3 ? version : 0,
    };

    /// <summary>Terminator length for a Micro QR version.</summary>
    public static int MicroTerminatorBits(int version) => (version * 2) + 1;
}
