# QrEncoder

**Class** in `Chuvadi.Barcodes.Encoders` (Encoders)

Encodes text as QR Code Model 2 symbols (versions 1–40), choosing modes optimally, with optional ECI, GS1 (FNC1) and structured append.

```csharp
public static class QrEncoder
```

## Methods

### `Encode`

__static__

```csharp
static QrSymbol Encode(string text, QrEncodeOptions? options = null)
```

Encodes `text` into a single QR Code symbol.

**Parameters**

- `text` — The data. May be empty.
- `options` — Options, or `null` for defaults.

**Returns:** The symbol. <exception cref="BarcodeEncodingException">The data does not fit or contains unsupported characters.</exception>

### `EncodeStructuredAppend`

__static__

```csharp
static IReadOnlyList<QrSymbol> EncodeStructuredAppend(string text, int symbolCount, QrEncodeOptions? options = null)
```

Splits `text` across `symbolCount` linked QR Code symbols (structured append). A reader reassembles them into the original text.

**Parameters**

- `text` — The data; must have at least `symbolCount` characters.
- `symbolCount` — Number of symbols, 2–16.
- `options` — Options applied to every symbol, or `null` for defaults.

**Returns:** The symbols, in sequence order. <exception cref="BarcodeEncodingException">A part does not fit or contains unsupported characters.</exception>

### `SplitForStructuredAppend`

__static__

```csharp
static IReadOnlyList<string> SplitForStructuredAppend(string text, int symbolCount)
```

Splits text for structured append exactly as `EncodeStructuredAppend` does, and returns the parts (useful for testing and for printing labels next to each symbol).

**Parameters**

- `text` — The data.
- `symbolCount` — Number of symbols, 2–16.

**Returns:** The text of each part.

---

_Source: [`src/Chuvadi.Barcodes.Encoders/QrEncoder.cs`](../../../src/Chuvadi.Barcodes.Encoders/QrEncoder.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
