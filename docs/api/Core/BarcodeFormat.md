# BarcodeFormat

**Enum** in `Chuvadi.Barcodes` (Core)

Every barcode symbology known to Chuvadi Barcodes. A format being listed here does not mean it is implemented yet; see the project README for the current coverage.

```csharp
public enum BarcodeFormat
```

## Values

| Name | Description |
|---|---|
| `QrCode` | QR Code Model 2 (ISO/IEC 18004), versions 1 to 40. |
| `QrCodeModel1` | QR Code Model 1 (obsolete; decode only). |
| `MicroQrCode` | Micro QR Code, versions M1 to M4. |
| `RectangularMicroQrCode` | Rectangular Micro QR Code (rMQR, ISO/IEC 23941). |
| `DataMatrix` | Data Matrix ECC 200 (ISO/IEC 16022). |
| `Pdf417` | PDF417 (ISO/IEC 15438). |
| `MicroPdf417` | MicroPDF417. |
| `Aztec` | Aztec Code (ISO/IEC 24778), including compact and rune symbols. |
| `MaxiCode` | MaxiCode (ISO/IEC 16023). |
| `HanXin` | Han Xin Code (ISO/IEC 20830). |
| `DotCode` | DotCode (AIM ISS DotCode). |
| `CodablockF` | Codablock F stacked symbology. |
| `Code16K` | Code 16K stacked symbology. |
| `Code49` | Code 49 stacked symbology. |
| `Code128` | Code 128, including GS1-128. |
| `Code39` | Code 39, standard and Full ASCII. |
| `Code93` | Code 93, standard and Full ASCII. |
| `Codabar` | Codabar. |
| `Ean13` | EAN-13. |
| `Ean8` | EAN-8. |
| `UpcA` | UPC-A. |
| `UpcE` | UPC-E. |
| `Itf` | Interleaved 2 of 5. |
| `Itf14` | ITF-14 (Interleaved 2 of 5 with bearer bars, GTIN-14). |
| `DataBarOmnidirectional` | GS1 DataBar Omnidirectional. |
| `DataBarTruncated` | GS1 DataBar Truncated. |
| `DataBarStacked` | GS1 DataBar Stacked. |
| `DataBarStackedOmnidirectional` | GS1 DataBar Stacked Omnidirectional. |
| `DataBarLimited` | GS1 DataBar Limited. |
| `DataBarExpanded` | GS1 DataBar Expanded. |
| `DataBarExpandedStacked` | GS1 DataBar Expanded Stacked. |
| `Gs1Composite` | GS1 Composite (a linear component with a CC-A, CC-B or CC-C 2D component). |
| `Code11` | Code 11. |
| `MsiPlessey` | MSI Plessey. |
| `Pharmacode` | Pharmacode (Laetus), one-track. |
| `PharmacodeTwoTrack` | Pharmacode (Laetus), two-track. |
| `Telepen` | Telepen. |
| `UspsIntelligentMail` | USPS Intelligent Mail barcode (IMb). |
| `RoyalMail4State` | Royal Mail 4-State Customer Code (RM4SCC). |
| `AustraliaPost` | Australia Post 4-State. |
| `JapanPost` | Japan Post 4-State. |
| `Kix` | Dutch KIX (Klant IndeX) 4-State. |

---

_Source: [`src/Chuvadi.Barcodes/BarcodeFormat.cs`](../../../src/Chuvadi.Barcodes/BarcodeFormat.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
