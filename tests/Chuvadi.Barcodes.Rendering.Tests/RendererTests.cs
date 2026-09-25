// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — SVG, raster and PNG rendering

using System.Text.RegularExpressions;
using Xunit;

namespace Chuvadi.Barcodes.Rendering.Tests;

public sealed class RendererTests
{
    private static readonly BitMatrix Sample = BitMatrix.Parse("110\n011\n000");

    [Fact]
    public void Svg_HasModuleViewBoxAndMergedRuns()
    {
        string svg = SvgRenderer.Render(Sample, new SvgRenderOptions { QuietZone = 2, ModuleSize = 10 });

        Assert.Contains("viewBox=\"0 0 7 7\"", svg, System.StringComparison.Ordinal);
        Assert.Contains("width=\"70\"", svg, System.StringComparison.Ordinal);
        Assert.Contains("M2 2h2v1h-2z", svg, System.StringComparison.Ordinal);
        Assert.Contains("M3 3h2v1h-2z", svg, System.StringComparison.Ordinal);
        Assert.Equal(2, Regex.Count(svg, "z"));
    }

    [Fact]
    public void Svg_TransparentBackgroundOmitsRect()
    {
        string svg = SvgRenderer.Render(Sample, new SvgRenderOptions { LightColor = null });

        Assert.DoesNotContain("<rect", svg, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Raster_UsesExactModuleSizeAndQuietZone()
    {
        LuminanceImage image = RasterRenderer.Render(Sample, new RasterRenderOptions { ModuleSize = 3, QuietZone = 1 });

        Assert.Equal(15, image.Width);
        Assert.Equal(15, image.Height);
        Assert.Equal(255, image[2, 2]);   // quiet zone
        Assert.Equal(0, image[3, 3]);     // module (0,0) dark
        Assert.Equal(0, image[8, 5]);     // module (1,0) dark
        Assert.Equal(255, image[9, 3]);   // module (2,0) light
        Assert.Equal(0, image[11, 6]);    // module (2,1) dark
    }

    [Fact]
    public void Png_StartsWithSignature()
    {
        byte[] png = PngRenderer.Render(Sample);

        Assert.Equal(0x89, png[0]);
        Assert.Equal((byte)'P', png[1]);
    }
}
