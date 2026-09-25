# MicroQrEncodeOptions

**Class** in `Chuvadi.Barcodes.Encoders` (Encoders)

Options for `MicroQrEncoder`. Available levels: M1 detection only (use L); M2 and M3 L or M; M4 L, M or Q. Level H does not exist in Micro QR.

```csharp
public sealed class MicroQrEncodeOptions
```

## Properties

### `ErrorCorrection`

```csharp
QrErrorCorrectionLevel ErrorCorrection
```

Error correction level (L, M or Q). Default `QrErrorCorrectionLevel.L`.

### `MinVersion`

```csharp
int MinVersion
```

Smallest version (1 = M1 … 4 = M4). Default 1.

### `MaxVersion`

```csharp
int MaxVersion
```

Largest version (1 = M1 … 4 = M4). Default 4.

### `Mask`

```csharp
int? Mask
```

Mask pattern 0–3, or `null` to pick the best mask. Default null.

### `CharacterSet`

```csharp
QrCharacterSet CharacterSet
```

Character set for byte-mode data: `QrCharacterSet.Auto` and `QrCharacterSet.Latin1` accept ISO-8859-1 only (Micro QR has no ECI); `QrCharacterSet.ShiftJis` enables Kanji mode. `QrCharacterSet.Utf8` writes UTF-8 bytes without an ECI. Default `QrCharacterSet.Auto`.

---

_Source: [`src/Chuvadi.Barcodes.Encoders/MicroQrEncodeOptions.cs`](../../../src/Chuvadi.Barcodes.Encoders/MicroQrEncodeOptions.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
