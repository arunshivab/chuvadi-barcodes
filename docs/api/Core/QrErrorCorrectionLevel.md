# QrErrorCorrectionLevel

**Enum** in `Chuvadi.Barcodes` (Core)

Error correction level for QR Code, Micro QR Code and rMQR symbols. Higher levels recover from more damage at the cost of a larger symbol.

```csharp
public enum QrErrorCorrectionLevel
```

## Values

| Name | Description |
|---|---|
| `L` | Level L: recovers roughly 7% of codewords. |
| `M` | Level M: recovers roughly 15% of codewords. |
| `Q` | Level Q: recovers roughly 25% of codewords. |
| `H` | Level H: recovers roughly 30% of codewords. |

---

_Source: [`src/Chuvadi.Barcodes/QrErrorCorrectionLevel.cs`](../../../src/Chuvadi.Barcodes/QrErrorCorrectionLevel.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
