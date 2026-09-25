# BarcodeEncodingException

**Class** in `Chuvadi.Barcodes.Encoders` (Encoders)

Thrown when data cannot be encoded: it is too long for the allowed symbol sizes, or it contains characters the symbology or chosen character set cannot represent.

```csharp
public sealed class BarcodeEncodingException : Exception
```

## Constructors

### `BarcodeEncodingException()`

Creates the exception with a default message.

### `BarcodeEncodingException(string message)`

Creates the exception with a message.

**Parameters**

- `message` — What went wrong.

### `BarcodeEncodingException(string message, Exception innerException)`

Creates the exception with a message and an inner exception.

**Parameters**

- `message` — What went wrong.
- `innerException` — The underlying cause.

---

_Source: [`src/Chuvadi.Barcodes.Encoders/BarcodeEncodingException.cs`](../../../src/Chuvadi.Barcodes.Encoders/BarcodeEncodingException.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
