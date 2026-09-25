# QrEncodeOptions

**Class** in `Chuvadi.Barcodes.Encoders` (Encoders)

Options for `QrEncoder`. Defaults produce the smallest valid symbol at level M.

```csharp
public sealed class QrEncodeOptions
```

## Properties

### `ErrorCorrection`

```csharp
QrErrorCorrectionLevel ErrorCorrection
```

Error correction level. Default `QrErrorCorrectionLevel.M`.

### `MinVersion`

```csharp
int MinVersion
```

Smallest version (1–40) the encoder may choose. Default 1.

### `MaxVersion`

```csharp
int MaxVersion
```

Largest version (1–40) the encoder may choose. Default 40.

### `Mask`

```csharp
int? Mask
```

Mask pattern 0–7, or `null` to pick the lowest-penalty mask. Default null.

### `BoostErrorCorrection`

```csharp
bool BoostErrorCorrection
```

When `true`, raises the error correction level as far as possible without increasing the version. Default `false`.

### `CharacterSet`

```csharp
QrCharacterSet CharacterSet
```

Character set for byte-mode data. Default `QrCharacterSet.Auto`.

### `EmitEci`

```csharp
bool EmitEci
```

Whether to emit ECI 26 when the data is UTF-8. Default `true` (standards-correct). Turn off only for readers that mishandle ECI.

### `Gs1`

```csharp
bool Gs1
```

Encodes a GS1 QR Code (FNC1 in first position). Separate variable-length element strings with the GS character U+001D. Default `false`.

---

_Source: [`src/Chuvadi.Barcodes.Encoders/QrEncodeOptions.cs`](../../../src/Chuvadi.Barcodes.Encoders/QrEncodeOptions.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
