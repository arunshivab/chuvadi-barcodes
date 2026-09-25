# 00 — Project Idea: Chuvadi.Barcodes

| Field | Value |
|-------|-------|
| Status | Accepted |
| Date | 2026-09-25 |
| Owner | Arun Shiva B |
| Repository | `github.com/arunshivab/chuvadi-barcodes` (public) |
| License | Apache-2.0 (same as Chuvadi PDF) |

## 1. The idea

A complete, open-source **barcode and QR code library for .NET 10** that both **creates**
(encodes) and **reads** (decodes) every major open 1D and 2D symbology — written from
scratch in pure managed C#, with **zero NuGet dependencies** in the shipped packages.

It belongs to the Chuvadi family (Chuvadi PDF, and later Sheets / Docs) but is a
**standalone library**: it does not depend on Chuvadi PDF, and anyone can use it on its
own — in a hospital system, a billing app, a web API, a desktop tool, or a PDF pipeline.

## 2. Why build it

- **One library, both directions.** Many .NET options either only generate or only read,
  or cover QR but not DataMatrix / PDF417 / GS1.
- **Zero dependencies.** No native binaries, no System.Drawing (Windows-only), no
  SkiaSharp. It runs the same on Windows, Linux, macOS, and WebAssembly.
- **Healthcare and India needs.** Patient wristbands, specimen labels, pharmacy (GS1
  DataMatrix), invoices (GST e-invoice QR), payments (UPI / Bharat QR), identity (Aadhaar
  Secure QR) — the payload formats matter as much as the symbols.
- **Vector-first output.** Symbols come out as exact module grids and bar widths, so they
  render razor-sharp in SVG and PDF at any print resolution.
- **Chuvadi integration.** A later thin adapter (`Chuvadi.Pdf.Barcodes`, in the
  chuvadi-pdf repo) will draw barcodes onto PDF pages and read barcodes found on them.

## 3. Scope decisions already taken

| # | Decision |
|---|----------|
| 1 | Standalone library in its own public repo, Chuvadi family naming (`Chuvadi.Barcodes.*`). |
| 2 | **Everything in v1**: all open symbologies listed in the design proposal, encode + decode. |
| 3 | Decoder must handle **all input qualities**: clean digital images, scanned documents, and camera photos. |
| 4 | **Our own image decoders** (PNG, JPEG, BMP, …) — no external imaging library. |
| 5 | **Open source for everyone** — Apache-2.0, matching Chuvadi PDF. |
| 6 | **Proprietary symbologies excluded**: Denso iQR, SQRC, Frame QR (no open specification). |
| 7 | Documentation is written at every step, under `docs/`. |

## 4. What success looks like for v1.0.0

1. Every symbology in scope encodes to a spec-correct symbol and decodes back
   (round-trip), for every size and error-correction level.
2. Real-world scans and phone photos from a maintained test corpus decode reliably.
3. Payload builders/parsers produce and read UPI, EMVCo, GS1, vCard, Wi-Fi, GST
   e-invoice and Aadhaar Secure QR content correctly.
4. Green CI on Windows, Linux and macOS; full XML docs; generated API docs.
5. Packages published (local feed first, nuget.org when ready).

## 5. What it is not

- Not a camera/scanner driver — it takes images (files or pixel buffers), not devices.
- Not a general image-editing library — the imaging code exists to feed the decoder and
  to write PNG output.
- Not an implementation of any proprietary symbology.
