# Chuvadi.Barcodes

**Pure .NET 10 barcode and QR code library — encode and decode every major open 1D and 2D
symbology. Zero dependencies. Part of the Chuvadi family.**

> **Status: under construction (0.x).** Milestone M1 (scaffold + CI) is in place. No
> symbology is implemented yet. See the [roadmap](#roadmap).

## Planned for v1.0.0

| Area | Coverage |
|------|----------|
| QR family | QR Model 2 (encode/decode), QR Model 1 (decode), Micro QR, rMQR, Structured Append |
| Other 2D | Data Matrix (incl. GS1), PDF417, MicroPDF417, Aztec, MaxiCode |
| 1D | Code 128 / GS1-128, Code 39, Code 93, Codabar, EAN-13/8, UPC-A/E, ITF / ITF-14, GS1 DataBar, Code 11, MSI Plessey, Pharmacode |
| Reading | Clean images, scanned documents, camera photos |
| Image formats | PNG, JPEG, BMP, GIF, TIFF, PNM — our own decoders, no external imaging library |
| Output | SVG, PNG, raw module grid / bar widths |
| Payloads | URL, vCard, Wi-Fi, SMS, email, geo, calendar, UPI, EMVCo / Bharat QR, GS1, GST e-invoice QR, Aadhaar Secure QR |

Full details: [design proposal](docs/design/01-design-proposal.md).

## Packages

| Package | Purpose |
|---------|---------|
| `Chuvadi.Barcodes` | Core types: formats, bit matrices, Reed–Solomon, GS1, pixel buffers, results |
| `Chuvadi.Barcodes.Encoders` | All encoders |
| `Chuvadi.Barcodes.Decoders` | All decoders |
| `Chuvadi.Barcodes.Imaging` | Image file readers/writers |
| `Chuvadi.Barcodes.Rendering` | SVG / PNG output |
| `Chuvadi.Barcodes.Payloads` | Typed payload builders and parsers |

## Building

```bash
dotnet build Chuvadi.Barcodes.slnx
dotnet test Chuvadi.Barcodes.slnx
```

Requires the .NET 10 SDK (pinned in `global.json`).

## Roadmap

| Milestone | Content | State |
|-----------|---------|-------|
| M0 | Project idea, design proposal, repository | Done |
| M1 | Scaffold + CI | Done |
| M2 | Core + all encoders + SVG/PNG rendering | Next |
| M3 | Image codecs | |
| M4 | Decoders — clean images | |
| M5 | Decoders — scans | |
| M6 | Decoders — camera photos | |
| M7 | Payloads | |
| M8 | Hardening → v1.0.0 | |
| M9 | `Chuvadi.Pdf.Barcodes` adapter (in chuvadi-pdf) | |

## Documentation

All project documentation lives in [`docs/`](docs/README.md); the generated API reference
is in [`docs/api/`](docs/api/README.md).

## License

[Apache-2.0](LICENSE).
