// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — rendering

namespace Chuvadi.Barcodes.Rendering;

/// <summary>Options for <see cref="SvgRenderer"/>.</summary>
public sealed class SvgRenderOptions
{
    /// <summary>Size of one module in SVG user units (the width/height attributes). Default 4.</summary>
    public double ModuleSize { get; init; } = 4;

    /// <summary>Quiet zone in modules on every side. Default 4 (QR Code); use 2 for Micro QR.</summary>
    public int QuietZone { get; init; } = 4;

    /// <summary>Colour of dark modules, as an SVG colour. Default <c>#000000</c>.</summary>
    public string DarkColor { get; init; } = "#000000";

    /// <summary>Background colour, or <see langword="null"/> for a transparent background. Default <c>#FFFFFF</c>.</summary>
    public string? LightColor { get; init; } = "#FFFFFF";
}
