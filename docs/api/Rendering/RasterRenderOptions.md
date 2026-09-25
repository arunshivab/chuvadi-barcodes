# RasterRenderOptions

**Class** in `Chuvadi.Barcodes.Rendering` (Rendering)

Options for `RasterRenderer` and `PngRenderer`.

```csharp
public sealed class RasterRenderOptions
```

## Properties

### `ModuleSize`

```csharp
int ModuleSize
```

Pixels per module (an exact integer, so no blurry scaling). Default 4.

### `QuietZone`

```csharp
int QuietZone
```

Quiet zone in modules on every side. Default 4 (QR Code); use 2 for Micro QR.

### `Dpi`

```csharp
int Dpi
```

Resolution recorded in the PNG file, or 0 to omit it. Default 0.

---

_Source: [`src/Chuvadi.Barcodes.Rendering/RasterRenderOptions.cs`](../../../src/Chuvadi.Barcodes.Rendering/RasterRenderOptions.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
