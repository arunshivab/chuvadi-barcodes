# SvgRenderOptions

**Class** in `Chuvadi.Barcodes.Rendering` (Rendering)

Options for `SvgRenderer`.

```csharp
public sealed class SvgRenderOptions
```

## Properties

### `ModuleSize`

```csharp
double ModuleSize
```

Size of one module in SVG user units (the width/height attributes). Default 4.

### `QuietZone`

```csharp
int QuietZone
```

Quiet zone in modules on every side. Default 4 (QR Code); use 2 for Micro QR.

### `DarkColor`

```csharp
string DarkColor
```

Colour of dark modules, as an SVG colour. Default `#000000`.

### `LightColor`

```csharp
string? LightColor
```

Background colour, or `null` for a transparent background. Default `#FFFFFF`.

---

_Source: [`src/Chuvadi.Barcodes.Rendering/SvgRenderOptions.cs`](../../../src/Chuvadi.Barcodes.Rendering/SvgRenderOptions.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
