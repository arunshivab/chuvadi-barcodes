// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — optimal mode segmentation (dynamic programming)

using System.Collections.Generic;

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>
/// Splits characters into mode segments that minimise the total bit length, using dynamic
/// programming over the four modes. Costs are kept in sixths of a bit so that numeric
/// (10 bits per 3 digits) and alphanumeric (11 bits per 2 characters) are exact.
/// </summary>
internal static class QrSegmenter
{
    private const int ModeCount = 4;

    /// <summary>
    /// Computes the optimal segmentation, or <see langword="null"/> when some character
    /// cannot be encoded in any allowed mode.
    /// </summary>
    /// <param name="characters">The input.</param>
    /// <param name="headerBits">Mode indicator + character-count bits for each mode (0 = mode not allowed).</param>
    public static List<QrSegment>? Segment(QrCharacter[] characters, int[] headerBits)
    {
        if (characters.Length == 0)
        {
            return [];
        }

        const long Infinity = long.MaxValue / 4;
        int n = characters.Length;
        long[] headCosts = new long[ModeCount];
        for (int m = 0; m < ModeCount; m++)
        {
            headCosts[m] = headerBits[m] > 0 ? headerBits[m] * 6L : Infinity;
        }

        // switchFrom[i][j]: when >= 0, the best path enters mode j for character i+1 by
        // switching from mode switchFrom[i][j] after character i; -1 means it stays in j.
        int[][] switchFrom = new int[n][];
        long[] prevCosts = (long[])headCosts.Clone();

        for (int i = 0; i < n; i++)
        {
            QrCharacter c = characters[i];
            long[] curCosts = [Infinity, Infinity, Infinity, Infinity];

            // Extend each mode with this character (staying in the same segment).
            for (int m = 0; m < ModeCount; m++)
            {
                if (prevCosts[m] >= Infinity || headerBits[m] == 0)
                {
                    continue;
                }

                long charCost = CharCost(c, (QrMode)m);
                if (charCost < 0)
                {
                    continue;
                }

                curCosts[m] = prevCosts[m] + charCost;
            }

            if (i < n - 1)
            {
                // Allow a mode switch after this character: new segment header in mode j.
                long[] next = (long[])curCosts.Clone();
                int[] switches = [-1, -1, -1, -1];
                for (int j = 0; j < ModeCount; j++)
                {
                    for (int k = 0; k < ModeCount; k++)
                    {
                        if (curCosts[k] >= Infinity || headCosts[j] >= Infinity || j == k)
                        {
                            continue;
                        }

                        long cost = RoundUpToBit(curCosts[k]) + headCosts[j];
                        if (cost < next[j])
                        {
                            next[j] = cost;
                            switches[j] = k;
                        }
                    }
                }

                prevCosts = next;
                switchFrom[i] = switches;
            }
            else
            {
                prevCosts = curCosts;
            }
        }

        int best = -1;
        for (int m = 0; m < ModeCount; m++)
        {
            if (prevCosts[m] < Infinity && (best < 0 || prevCosts[m] < prevCosts[best]))
            {
                best = m;
            }
        }

        if (best < 0)
        {
            return null;
        }

        // Trace back: mode of each character.
        QrMode[] modes = new QrMode[n];
        int mode = best;
        for (int i = n - 1; i >= 0; i--)
        {
            modes[i] = (QrMode)mode;
            if (i > 0)
            {
                // Character i entered mode `mode` either by extension (same mode at i-1)
                // or by a switch recorded after character i-1.
                int[] sw = switchFrom[i - 1];
                if (sw[mode] >= 0)
                {
                    mode = sw[mode];
                }
            }
        }

        List<QrSegment> segments = [];
        int start = 0;
        for (int i = 1; i <= n; i++)
        {
            if (i == n || modes[i] != modes[start])
            {
                segments.Add(new QrSegment(modes[start], [.. characters[start..i]]));
                start = i;
            }
        }

        return segments;
    }

    private static long RoundUpToBit(long sixths) => (sixths + 5) / 6 * 6;

    private static long CharCost(QrCharacter c, QrMode mode) => mode switch
    {
        QrMode.Numeric => c.NumericValue >= 0 ? 20 : -1,
        QrMode.Alphanumeric => c.AlphanumericValues.Length > 0 ? 33L * c.AlphanumericValues.Length : -1,
        QrMode.Byte => 48L * c.Bytes.Length,
        _ => c.KanjiValue >= 0 ? 78 : -1,
    };
}
