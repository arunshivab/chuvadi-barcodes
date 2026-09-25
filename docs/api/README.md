# API Reference

Auto-generated from XML doc comments. One file per public type, grouped by module.

Regenerate with:

```bash
python tools/gen_api_docs.py
```

## Chuvadi.Barcodes

| Type | Kind | Description |
|---|---|---|
| [BarcodeFormat](Core/BarcodeFormat.md) | enum | Every barcode symbology known to Chuvadi Barcodes. |
| [BitMatrix](Core/BitMatrix.md) | class | A two-dimensional grid of modules, each either dark (`true`) or light (`false`). |
| [LuminanceImage](Core/LuminanceImage.md) | class | An 8-bit greyscale image: one byte per pixel, row-major, no padding between rows. 0 is black and 255 is white. |
| [QrErrorCorrectionLevel](Core/QrErrorCorrectionLevel.md) | enum | Error correction level for QR Code, Micro QR Code and rMQR symbols. |

## Chuvadi.Barcodes.Encoders

| Type | Kind | Description |
|---|---|---|
| [BarcodeEncodingException](Encoders/BarcodeEncodingException.md) | class | Thrown when data cannot be encoded: it is too long for the allowed symbol sizes, or it contains characters the symbology or chosen character set cannot represent. |
| [MicroQrEncodeOptions](Encoders/MicroQrEncodeOptions.md) | class | Options for `MicroQrEncoder`. |
| [MicroQrEncoder](Encoders/MicroQrEncoder.md) | class | Encodes text as Micro QR symbols (M1–M4). |
| [QrCharacterSet](Encoders/QrCharacterSet.md) | enum | How text is turned into bytes for the byte (and Kanji) modes of QR-family symbols. |
| [QrEncodeOptions](Encoders/QrEncodeOptions.md) | class | Options for `QrEncoder`. |
| [QrEncoder](Encoders/QrEncoder.md) | class | Encodes text as QR Code Model 2 symbols (versions 1–40), choosing modes optimally, with optional ECI, GS1 (FNC1) and structured append. |
| [QrSymbol](Encoders/QrSymbol.md) | class | An encoded QR-family symbol: its module grid plus the parameters chosen. |

## Chuvadi.Barcodes.Imaging

| Type | Kind | Description |
|---|---|---|
| [PngWriter](Imaging/PngWriter.md) | class | Writes PNG files. |

## Chuvadi.Barcodes.Rendering

| Type | Kind | Description |
|---|---|---|
| [PngRenderer](Rendering/PngRenderer.md) | class | Renders a `BitMatrix` to a PNG file. |
| [RasterRenderOptions](Rendering/RasterRenderOptions.md) | class | Options for `RasterRenderer` and `PngRenderer`. |
| [RasterRenderer](Rendering/RasterRenderer.md) | class | Renders a `BitMatrix` to an 8-bit greyscale image (dark = 0, light = 255). |
| [SvgRenderOptions](Rendering/SvgRenderOptions.md) | class | Options for `SvgRenderer`. |
| [SvgRenderer](Rendering/SvgRenderer.md) | class | Renders a `BitMatrix` as a compact SVG document. |
