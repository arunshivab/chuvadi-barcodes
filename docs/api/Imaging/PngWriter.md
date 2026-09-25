# PngWriter

**Class** in `Chuvadi.Barcodes.Imaging` (Imaging)

Writes PNG files. Uses only the .NET base library (zlib via `ZLibStream`).

```csharp
public static class PngWriter
```

## Methods

### `WriteGray8`

__static__

```csharp
static byte[] WriteGray8(int width, int height, ReadOnlySpan<byte> pixels, int dpi = 0)
```

Encodes an 8-bit greyscale image as PNG.

**Parameters**

- `width` — Width in pixels; must be positive.
- `height` — Height in pixels; must be positive.
- `pixels` — Row-major pixels, exactly `width` × `height` bytes (0 = black).
- `dpi` — Resolution to record in a pHYs chunk, or 0 to omit it.

**Returns:** The PNG file bytes.

---

_Source: [`src/Chuvadi.Barcodes.Imaging/PngWriter.cs`](../../../src/Chuvadi.Barcodes.Imaging/PngWriter.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
