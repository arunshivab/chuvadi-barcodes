# RasterRenderer

**Class** in `Chuvadi.Barcodes.Rendering` (Rendering)

Renders a `BitMatrix` to an 8-bit greyscale image (dark = 0, light = 255).

```csharp
public static class RasterRenderer
```

## Methods

### `Render`

__static__

```csharp
static LuminanceImage Render(BitMatrix matrix, RasterRenderOptions? options = null)
```

Renders `matrix` with an integer module size and quiet zone.

**Parameters**

- `matrix` — The symbol modules.
- `options` — Options, or `null` for defaults.

**Returns:** The image.

---

_Source: [`src/Chuvadi.Barcodes.Rendering/RasterRenderer.cs`](../../../src/Chuvadi.Barcodes.Rendering/RasterRenderer.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
