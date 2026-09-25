// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

using System;

namespace Chuvadi.Barcodes;

/// <summary>
/// An 8-bit greyscale image: one byte per pixel, row-major, no padding between rows.
/// 0 is black and 255 is white. This is the pixel format the renderers produce and the
/// decoders consume.
/// </summary>
public sealed class LuminanceImage
{
    private readonly byte[] _pixels;

    /// <summary>Wraps an existing pixel buffer (not copied).</summary>
    /// <param name="width">Width in pixels; must be positive.</param>
    /// <param name="height">Height in pixels; must be positive.</param>
    /// <param name="pixels">Exactly <paramref name="width"/> × <paramref name="height"/> bytes.</param>
    public LuminanceImage(int width, int height, byte[] pixels)
    {
        ArgumentNullException.ThrowIfNull(pixels);
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        if (pixels.Length != (long)width * height)
        {
            throw new ArgumentException($"Expected {(long)width * height} bytes, got {pixels.Length}.", nameof(pixels));
        }

        Width = width;
        Height = height;
        _pixels = pixels;
    }

    /// <summary>Width in pixels.</summary>
    public int Width { get; }

    /// <summary>Height in pixels.</summary>
    public int Height { get; }

    /// <summary>The pixel bytes, row-major.</summary>
    public ReadOnlyMemory<byte> Pixels => _pixels;

    /// <summary>Gets the luminance at the given pixel.</summary>
    /// <param name="x">Column.</param>
    /// <param name="y">Row.</param>
    /// <returns>0 (black) to 255 (white).</returns>
    public byte this[int x, int y]
    {
        get
        {
            if ((uint)x >= (uint)Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if ((uint)y >= (uint)Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            return _pixels[(y * Width) + x];
        }
    }
}
