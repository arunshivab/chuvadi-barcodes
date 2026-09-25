// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — raster rendering

using System;

namespace Chuvadi.Barcodes.Rendering;

/// <summary>Renders a <see cref="BitMatrix"/> to an 8-bit greyscale image (dark = 0, light = 255).</summary>
public static class RasterRenderer
{
    /// <summary>Renders <paramref name="matrix"/> with an integer module size and quiet zone.</summary>
    /// <param name="matrix">The symbol modules.</param>
    /// <param name="options">Options, or <see langword="null"/> for defaults.</param>
    /// <returns>The image.</returns>
    public static LuminanceImage Render(BitMatrix matrix, RasterRenderOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        RasterRenderOptions opts = options ?? new RasterRenderOptions();
        if (opts.ModuleSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "ModuleSize must be at least 1.");
        }

        if (opts.QuietZone < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "QuietZone must not be negative.");
        }

        int s = opts.ModuleSize;
        int q = opts.QuietZone * s;
        int width = (matrix.Width * s) + (2 * q);
        int height = (matrix.Height * s) + (2 * q);
        byte[] pixels = new byte[width * height];
        Array.Fill(pixels, (byte)255);

        for (int my = 0; my < matrix.Height; my++)
        {
            for (int mx = 0; mx < matrix.Width; mx++)
            {
                if (!matrix[mx, my])
                {
                    continue;
                }

                for (int py = 0; py < s; py++)
                {
                    int rowStart = ((q + (my * s) + py) * width) + q + (mx * s);
                    pixels.AsSpan(rowStart, s).Clear();
                }
            }
        }

        return new LuminanceImage(width, height, pixels);
    }
}
