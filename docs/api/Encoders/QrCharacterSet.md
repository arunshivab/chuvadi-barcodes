# QrCharacterSet

**Enum** in `Chuvadi.Barcodes.Encoders` (Encoders)

How text is turned into bytes for the byte (and Kanji) modes of QR-family symbols.

```csharp
public enum QrCharacterSet
```

## Values

| Name | Description |
|---|---|
| `Auto` | ISO-8859-1 when every character fits (the QR default, no ECI needed); otherwise UTF-8, announced with ECI 26 unless `QrEncodeOptions.EmitEci` is off. |
| `Latin1` | ISO-8859-1 (Latin-1). Characters above U+00FF are rejected. |
| `Utf8` | UTF-8, announced with ECI 26 unless `QrEncodeOptions.EmitEci` is off. |
| `ShiftJis` | Shift JIS. Double-byte characters use the compact Kanji mode; no ECI is emitted. Characters outside Shift JIS are rejected. |

---

_Source: [`src/Chuvadi.Barcodes.Encoders/QrCharacterSet.cs`](../../../src/Chuvadi.Barcodes.Encoders/QrCharacterSet.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
