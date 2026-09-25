// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — rendering

namespace Chuvadi.Barcodes.Rendering;

/// <summary>Options for <see cref="RasterRenderer"/> and <see cref="PngRenderer"/>.</summary>
public sealed class RasterRenderOptions
{
    /// <summary>Pixels per module (an exact integer, so no blurry scaling). Default 4.</summary>
    public int ModuleSize { get; init; } = 4;

    /// <summary>Quiet zone in modules on every side. Default 4 (QR Code); use 2 for Micro QR.</summary>
    public int QuietZone { get; init; } = 4;

    /// <summary>Resolution recorded in the PNG file, or 0 to omit it. Default 0.</summary>
    public int Dpi { get; init; }
}
