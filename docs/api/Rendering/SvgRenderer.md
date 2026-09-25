# SvgRenderer

**Class** in `Chuvadi.Barcodes.Rendering` (Rendering)

Renders a `BitMatrix` as a compact SVG document. The view box is measured in modules, so the symbol stays exact at any display size. Horizontal runs of dark modules are merged into single path rectangles.

```csharp
public static class SvgRenderer
```

## Methods

### `Render`

__static__

```csharp
static string Render(BitMatrix matrix, SvgRenderOptions? options = null)
```

Renders `matrix` to an SVG document string.

**Parameters**

- `matrix` — The symbol modules.
- `options` — Options, or `null` for defaults.

**Returns:** The SVG markup (UTF-8 text, no BOM needed).

---

_Source: [`src/Chuvadi.Barcodes.Rendering/SvgRenderer.cs`](../../../src/Chuvadi.Barcodes.Rendering/SvgRenderer.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
