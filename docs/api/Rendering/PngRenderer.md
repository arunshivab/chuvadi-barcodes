# PngRenderer

**Class** in `Chuvadi.Barcodes.Rendering` (Rendering)

Renders a `BitMatrix` to a PNG file.

```csharp
public static class PngRenderer
```

## Methods

### `Render`

__static__

```csharp
static byte[] Render(BitMatrix matrix, RasterRenderOptions? options = null)
```

Renders `matrix` to PNG bytes.

**Parameters**

- `matrix` — The symbol modules.
- `options` — Options, or `null` for defaults.

**Returns:** The PNG file bytes.

---

_Source: [`src/Chuvadi.Barcodes.Rendering/PngRenderer.cs`](../../../src/Chuvadi.Barcodes.Rendering/PngRenderer.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
