// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — independent decoder used as a test oracle (ZXing.Net, test-only, D-016)

using System.Collections.Generic;
using System.Text;
using Chuvadi.Barcodes.Rendering;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Chuvadi.Barcodes.Encoders.Tests;

internal static class ZxingOracle
{
    static ZxingOracle()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static Result? DecodeQr(BitMatrix matrix, string? characterSet = null)
    {
        LuminanceImage image = RasterRenderer.Render(matrix, new RasterRenderOptions
        {
            ModuleSize = 4,
            QuietZone = 4,
        });
        RGBLuminanceSource source = new(image.Pixels.ToArray(), image.Width, image.Height, RGBLuminanceSource.BitmapFormat.Gray8);
        BinaryBitmap bitmap = new(new HybridBinarizer(source));
        Dictionary<DecodeHintType, object> hints = new()
        {
            [DecodeHintType.PURE_BARCODE] = true,
        };
        if (characterSet is not null)
        {
            hints[DecodeHintType.CHARACTER_SET] = characterSet;
        }

        return new QRCodeReader().decode(bitmap, hints);
    }
}
