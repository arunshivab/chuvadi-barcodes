# MicroQrEncoder

**Class** in `Chuvadi.Barcodes.Encoders` (Encoders)

Encodes text as Micro QR symbols (M1–M4). Micro QR has no ECI, GS1 or structured append.

```csharp
public static class MicroQrEncoder
```

## Methods

### `Encode`

__static__

```csharp
static QrSymbol Encode(string text, MicroQrEncodeOptions? options = null)
```

Encodes `text` into the smallest Micro QR symbol that fits.

**Parameters**

- `text` — The data.
- `options` — Options, or `null` for defaults.

**Returns:** The symbol. <exception cref="BarcodeEncodingException">The data does not fit or contains unsupported characters.</exception>

---

_Source: [`src/Chuvadi.Barcodes.Encoders/MicroQrEncoder.cs`](../../../src/Chuvadi.Barcodes.Encoders/MicroQrEncoder.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
