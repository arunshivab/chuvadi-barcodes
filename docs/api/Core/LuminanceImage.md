# LuminanceImage

**Class** in `Chuvadi.Barcodes` (Core)

An 8-bit greyscale image: one byte per pixel, row-major, no padding between rows. 0 is black and 255 is white. This is the pixel format the renderers produce and the decoders consume.

```csharp
public sealed class LuminanceImage
```

## Constructors

### `LuminanceImage(int width, int height, byte[] pixels)`

Wraps an existing pixel buffer (not copied).

**Parameters**

- `width` — Width in pixels; must be positive.
- `height` — Height in pixels; must be positive.
- `pixels` — Exactly `width` × `height` bytes.

## Properties

### `Width`

```csharp
int Width
```

Width in pixels.

### `Height`

```csharp
int Height
```

Height in pixels.

### `Pixels`

```csharp
ReadOnlyMemory<byte> Pixels => _pixels
```

The pixel bytes, row-major.

---

_Source: [`src/Chuvadi.Barcodes/LuminanceImage.cs`](../../../src/Chuvadi.Barcodes/LuminanceImage.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
