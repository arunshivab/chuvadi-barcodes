# QrSymbol

**Class** in `Chuvadi.Barcodes.Encoders` (Encoders)

An encoded QR-family symbol: its module grid plus the parameters chosen.

```csharp
public sealed class QrSymbol
```

## Properties

### `Format`

```csharp
BarcodeFormat Format
```

`BarcodeFormat.QrCode` or `BarcodeFormat.MicroQrCode`.

### `Version`

```csharp
int Version
```

Symbol version: 1–40 for QR Code, 1–4 (M1–M4) for Micro QR.

### `ErrorCorrection`

```csharp
QrErrorCorrectionLevel ErrorCorrection
```

Error correction level used.

### `Mask`

```csharp
int Mask
```

Mask pattern applied (0–7 for QR Code, 0–3 for Micro QR).

### `Matrix`

```csharp
BitMatrix Matrix
```

The modules, without quiet zone.

### `QuietZone`

```csharp
int QuietZone => Format == BarcodeFormat.MicroQrCode ? 2 : 4
```

Quiet zone width in modules required around the symbol: 4 for QR Code, 2 for Micro QR.

### `StructuredAppendIndex`

```csharp
int StructuredAppendIndex
```

Zero-based position within a structured-append sequence, or -1 when not part of one.

### `StructuredAppendCount`

```csharp
int StructuredAppendCount
```

Number of symbols in the structured-append sequence, or 0 when not part of one.

---

_Source: [`src/Chuvadi.Barcodes.Encoders/QrSymbol.cs`](../../../src/Chuvadi.Barcodes.Encoders/QrSymbol.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
