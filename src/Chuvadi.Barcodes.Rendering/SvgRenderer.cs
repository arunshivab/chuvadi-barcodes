// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — SVG rendering

using System;
using System.Globalization;
using System.Text;

namespace Chuvadi.Barcodes.Rendering;

/// <summary>
/// Renders a <see cref="BitMatrix"/> as a compact SVG document. The view box is measured in
/// modules, so the symbol stays exact at any display size. Horizontal runs of dark modules
/// are merged into single path rectangles.
/// </summary>
public static class SvgRenderer
{
    /// <summary>Renders <paramref name="matrix"/> to an SVG document string.</summary>
    /// <param name="matrix">The symbol modules.</param>
    /// <param name="options">Options, or <see langword="null"/> for defaults.</param>
    /// <returns>The SVG markup (UTF-8 text, no BOM needed).</returns>
    public static string Render(BitMatrix matrix, SvgRenderOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        SvgRenderOptions opts = options ?? new SvgRenderOptions();
        ArgumentOutOfRangeException.ThrowIfNegative(opts.QuietZone, nameof(options));
        if (opts.ModuleSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "ModuleSize must be positive.");
        }

        int q = opts.QuietZone;
        int w = matrix.Width + (2 * q);
        int h = matrix.Height + (2 * q);
        CultureInfo ci = CultureInfo.InvariantCulture;

        StringBuilder sb = new();
        sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\"");
        sb.Append(ci, $" width=\"{w * opts.ModuleSize}\" height=\"{h * opts.ModuleSize}\"");
        sb.Append(ci, $" viewBox=\"0 0 {w} {h}\" shape-rendering=\"crispEdges\">\n");
        if (opts.LightColor is not null)
        {
            sb.Append(ci, $"<rect width=\"{w}\" height=\"{h}\" fill=\"{Escape(opts.LightColor)}\"/>\n");
        }

        sb.Append(ci, $"<path fill=\"{Escape(opts.DarkColor)}\" d=\"");
        for (int y = 0; y < matrix.Height; y++)
        {
            int x = 0;
            while (x < matrix.Width)
            {
                if (!matrix[x, y])
                {
                    x++;
                    continue;
                }

                int start = x;
                while (x < matrix.Width && matrix[x, y])
                {
                    x++;
                }

                sb.Append(ci, $"M{start + q} {y + q}h{x - start}v1h-{x - start}z");
            }
        }

        sb.Append("\"/>\n</svg>\n");
        return sb.ToString();
    }

    private static string Escape(string value) =>
        value.Replace("&", "&amp;", StringComparison.Ordinal)
             .Replace("\"", "&quot;", StringComparison.Ordinal)
             .Replace("<", "&lt;", StringComparison.Ordinal);
}
