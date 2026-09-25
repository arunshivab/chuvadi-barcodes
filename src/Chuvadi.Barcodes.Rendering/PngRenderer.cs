// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — PNG rendering

using System;
using Chuvadi.Barcodes.Imaging;

namespace Chuvadi.Barcodes.Rendering;

/// <summary>Renders a <see cref="BitMatrix"/> to a PNG file.</summary>
public static class PngRenderer
{
    /// <summary>Renders <paramref name="matrix"/> to PNG bytes.</summary>
    /// <param name="matrix">The symbol modules.</param>
    /// <param name="options">Options, or <see langword="null"/> for defaults.</param>
    /// <returns>The PNG file bytes.</returns>
    public static byte[] Render(BitMatrix matrix, RasterRenderOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        RasterRenderOptions opts = options ?? new RasterRenderOptions();
        LuminanceImage image = RasterRenderer.Render(matrix, opts);
        return PngWriter.WriteGray8(image.Width, image.Height, image.Pixels.Span, opts.Dpi);
    }
}
