# 01 — Design Proposal: Chuvadi.Barcodes v1

| Field | Value |
|-------|-------|
| Status | Draft — for review |
| Date | 2026-09-25 |
| Owner | Arun Shiva B |
| Depends on | [00 — Project idea](00-project-idea.md) |
| Amended by | D-010 — `Encoding`/`Decoding` projects renamed to `Encoders`/`Decoders` (see [02 — M1](02-m1-scaffold.md)) |

All type and member names in this document are **proposed**. Nothing exists in code yet.
Names will be finalised in the milestone design notes (02, 03, …) and recorded in the
[decision log](DECISIONS.md).

---

## 1. Goals and non-goals

### Goals
- Encode and decode every open symbology in section 3, in pure managed C# (.NET 10).
- Zero NuGet dependencies in `src/`; test projects may use test packages only.
- Cross-platform and WASM-safe: no native code, no P/Invoke, no System.Drawing,
  trimming- and AOT-friendly (no reflection-based serialization).
- Deterministic output: the same input and options always give the same symbol bytes.
- Same engineering standard as Chuvadi PDF: nullable enabled, warnings as errors,
  analyzers at latest, XML docs on every public member, generated API docs checked in CI.

### Non-goals
- Proprietary symbologies (Denso iQR, SQRC, Frame QR).
- Device access (cameras, scanners, printers).
- General-purpose image processing beyond what decoding and PNG output need.

---

## 2. Terminology

| Term | Meaning |
|------|---------|
| Symbology | A barcode type (QR, Code 128, DataMatrix, …). |
| Symbol | One printed/rendered barcode. |
| Module | The smallest square (2D) or narrowest bar/space unit (1D). |
| Quiet zone | Required blank margin around a symbol. |
| EC | Error correction (Reed–Solomon in most 2D codes). |
| ECI | Extended Channel Interpretation — tells the reader which character set the data uses. |
| GS1 | Standards body for product/healthcare identifiers; defines Application Identifiers (AIs) such as (01) GTIN, (17) expiry, (10) batch. |
| Payload | What the decoded text *means* (a URL, a UPI payment, a GS1 element string, …). |

---

## 3. Symbology scope for v1

### 3.1 QR family (ISO/IEC 18004, ISO/IEC 23941)

| Symbology | Encode | Decode | Notes |
|-----------|:------:|:------:|-------|
| QR Code Model 2 | ✅ | ✅ | Versions 1–40, EC L/M/Q/H, numeric/alphanumeric/byte/Kanji modes, ECI, FNC1 (GS1 QR) |
| QR Code Model 1 | — | ✅ | Obsolete; decode only so old labels can still be read |
| Micro QR | ✅ | ✅ | M1–M4 |
| rMQR (Rectangular Micro QR) | ✅ | ✅ | All 32 sizes |
| Structured Append | ✅ | ✅ | Split one message across up to 16 QR symbols, and reassemble |

### 3.2 Other 2D symbologies

| Symbology | Encode | Decode | Notes |
|-----------|:------:|:------:|-------|
| Data Matrix ECC 200 | ✅ | ✅ | Square and rectangular, all encodation schemes (ASCII, C40, Text, X12, EDIFACT, Base256), GS1 DataMatrix, Structured Append |
| PDF417 | ✅ | ✅ | EC levels 0–8, Macro PDF417, compact/truncated |
| MicroPDF417 | ✅ | ✅ | |
| Aztec Code | ✅ | ✅ | Compact, full-range, Aztec Runes |
| MaxiCode | ✅ | ✅ | Modes 2–6 (UPS/courier) |

### 3.3 1D (linear) symbologies

| Symbology | Encode | Decode | Notes |
|-----------|:------:|:------:|-------|
| Code 128 | ✅ | ✅ | Code sets A/B/C with optimal switching; GS1-128 |
| Code 39 | ✅ | ✅ | Standard + Full ASCII, optional check digit |
| Code 93 | ✅ | ✅ | Standard + Full ASCII |
| Codabar | ✅ | ✅ | |
| EAN-13 / EAN-8 | ✅ | ✅ | With 2- and 5-digit add-ons |
| UPC-A / UPC-E | ✅ | ✅ | With add-ons |
| ITF (Interleaved 2 of 5) / ITF-14 | ✅ | ✅ | Bearer bars for ITF-14 |
| GS1 DataBar | ✅ | ✅ | Omnidirectional, Truncated, Stacked, Stacked Omni, Limited, Expanded, Expanded Stacked |
| Code 11 | ✅ | ✅ | Telecom labelling |
| MSI Plessey | ✅ | ✅ | Mod 10 / Mod 11 / Mod 10+10 check variants |
| Pharmacode (Laetus, one-track) | ✅ | ✅ | Pharmaceutical packaging control code |

### 3.4 Candidates to confirm (open question Q1)

These are open standards but less common. Listed so the "everything" decision is explicit:

- **GS1 Composite** (CC-A / CC-B / CC-C — a 2D component stacked on a linear barcode)
- **Han Xin Code** (ISO/IEC 20830 — Chinese)
- **DotCode** (AIM — high-speed printing, tobacco)
- **Codablock F, Code 16K, Code 49** (legacy stacked codes)
- **Pharmacode two-track**, **Telepen**, postal codes (**USPS IMb**, **Royal Mail 4-State**, **Australia Post**, **Japan Post**, **KIX**)

---

## 4. Repository and project layout

```
chuvadi-barcodes/
  Chuvadi.Barcodes.slnx
  Directory.Build.props          shared build settings (mirrors Chuvadi PDF)
  Directory.Packages.props       central package versions — test packages only
  global.json                    .NET SDK pin
  .editorconfig / .gitattributes
  LICENSE (Apache-2.0) · README.md · CHANGELOG.md · CONTRIBUTING.md · CLAUDE.md
  .github/workflows/             CI: style, docs-up-to-date, build & test matrix
  build/pack.ps1                 mono-versioned pack of all packages
  tools/                         check_style.py, gen_api_docs.py
  docs/
    README.md  setup/  design/  api/ (generated)
  src/
    Chuvadi.Barcodes             core types (no dependencies)
    Chuvadi.Barcodes.Encoding    all encoders
    Chuvadi.Barcodes.Decoding    all decoders
    Chuvadi.Barcodes.Imaging     image file readers/writers
    Chuvadi.Barcodes.Rendering   SVG / PNG output
    Chuvadi.Barcodes.Payloads    typed payload builders and parsers
  tests/
    one test project per src project
    Chuvadi.Barcodes.Conformance round-trip and corpus tests across all projects
  corpus/                        real-world scans and photos for decoder tests
  examples/
    Chuvadi.Barcodes.Cli         command-line encode/decode tool (for visual checks)
  benchmarks/
```

One project = one namespace, as in Chuvadi PDF's rendering projects.

### 4.1 Dependency graph (acyclic)

```
                 Chuvadi.Barcodes  (core)
             ┌──────────┼───────────┬──────────────┐
         Encoding    Decoding    Payloads      Imaging (leaf, independent)
             └──────┬───┘
                Rendering ──────► Imaging (for PNG writing)
```

- **Core** holds shared value types, bit containers, Reed–Solomon, GS1 and ECI tables,
  and the **pixel/luminance buffer** type. Putting the buffer in core means the decoder
  never depends on file formats.
- **Imaging** turns files into pixel buffers (and back). It does not know about barcodes.
- **Decoding** takes a luminance buffer — from Imaging, from a PDF renderer, or from the
  caller's own camera frame.
- **Payloads** works on text/bytes only; it never touches images.

### 4.2 Packages

Every `src/` project ships as its own NuGet package, **mono-versioned** (all packages
share one version), plus possibly a convenience meta-package `Chuvadi.Barcodes.All`
(open question Q4). Package IDs `Chuvadi.Barcodes` and `Chuvadi.Barcodes.*` were free on
nuget.org when checked on 2026-09-25.

---

## 5. Core (`Chuvadi.Barcodes`)

| Area | Content |
|------|---------|
| Formats | `BarcodeFormat` enum covering every symbology in section 3 |
| Bits | `BitArray`, `BitMatrix` (packed, row-major), `BitWriter` / `BitReader` |
| Error correction | Generic Reed–Solomon over Galois fields: GF(256) with the field polynomials each symbology uses (QR, DataMatrix, Aztec 8-bit, MaxiCode 6-bit), GF(929) for PDF417, GF(16)/GF(64)/GF(1024)/GF(4096) for Aztec; encoder + error-and-erasure decoder |
| Other checks | BCH codes (QR format/version info), mod-10/mod-11/mod-43/mod-47/mod-103 check digits |
| Character sets | ECI assignment table, ISO-8859-x, Shift-JIS (for QR Kanji — implemented as tables, since Shift-JIS is not guaranteed on every .NET runtime), UTF-8 |
| GS1 | Application Identifier table, FNC1 handling, element-string parse/validate |
| Imaging bridge | `LuminanceImage` (8-bit grey, width, height, stride) and `RgbaImage` |
| Results | `BarcodeResult` — format, text, raw bytes, ECI, corner points, EC level/version, GS1 flag, structured-append info, error count corrected |

---

## 6. Encoding (`Chuvadi.Barcodes.Encoding`)

**Proposed shape**

```csharp
BarcodeSymbol symbol = BarcodeEncoder.Encode(BarcodeFormat.QrCode, "https://example.org",
    new QrEncodeOptions { ErrorCorrection = QrErrorCorrection.M });
```

- 2D encoders return a `BitMatrix` plus metadata (version, EC level, mask).
- 1D encoders return a **bar/space width pattern** (and human-readable text), not a
  bitmap — renderers decide pixel sizes. Stacked codes (PDF417, DataBar Stacked) return
  a matrix.
- Each symbology gets its own options type (QR version/EC/mask/min-version, DataMatrix
  size/shape, PDF417 columns/EC level, …). Sensible defaults: smallest symbol that fits.
- **Optimal segmentation**: QR picks mode switches (numeric/alphanumeric/byte/Kanji)
  to minimise symbol size; Code 128 picks code-set switches; DataMatrix picks
  encodation schemes.

---

## 7. Imaging (`Chuvadi.Barcodes.Imaging`)

Our own readers, pure managed, using only the .NET base library (for example
`System.IO.Compression.ZLibStream` for PNG's deflate — part of .NET itself, not a NuGet
package).

| Format | Read | Write | Scope |
|--------|:----:|:-----:|-------|
| PNG | ✅ | ✅ | All colour types and bit depths, interlaced (Adam7), palette, transparency |
| JPEG | ✅ | — | Baseline and progressive, 4:4:4 / 4:2:2 / 4:2:0 subsampling, restart markers, EXIF orientation |
| BMP | ✅ | ✅ | 1/4/8/24/32-bit, RLE4/RLE8, top-down and bottom-up |
| GIF | ✅ | — | First frame (LZW) |
| TIFF | ✅ | — | Uncompressed, PackBits, LZW, Deflate, CCITT G3/G4 (common for fax/scans) |
| PNM (PBM/PGM/PPM) | ✅ | ✅ | Handy for tests |

**Honest gap:** JPEG (especially progressive) and TIFF CCITT G3/G4 are each substantial.
**WebP** and **HEIC/HEIF** (iPhone photos) are excluded from v1: WebP is large, and
HEIC depends on HEVC, which is patent-encumbered. Callers can convert those to JPEG/PNG,
or hand us raw pixels. See open question Q2.

---

## 8. Decoding (`Chuvadi.Barcodes.Decoding`)

**Proposed shape**

```csharp
IReadOnlyList<BarcodeResult> results = BarcodeReader.Read(image,
    new ReadOptions { Formats = BarcodeFormats.All, Effort = ReadEffort.Thorough });
```

### 8.1 Pipeline

```
pixels ─► luminance ─► binarise ─► locate ─► sample grid ─► bits ─► error-correct ─► decode data ─► result
```

| Stage | Techniques |
|-------|------------|
| Luminance | RGB → grey; optional inversion for light-on-dark symbols; mirrored-image retry |
| Binarise | Global histogram (clean images), local adaptive thresholding (uneven lighting, shadows) — chosen automatically, both tried at higher effort |
| Locate | QR: 1:1:3:1:1 finder patterns. Micro QR / rMQR: single finder + timing. DataMatrix: L-shaped finder + clock track. Aztec / MaxiCode: bullseye. PDF417: start/stop patterns. 1D: row scans at several heights and angles |
| Geometry | Perspective (homography) transform from detected corners; alignment-pattern refinement for large QR; tolerance for curvature on bottles/wristbands at higher effort |
| Sample | Read module centres through the transform into a `BitMatrix` |
| Error-correct | Reed–Solomon errors + erasures; report how many were corrected |
| Decode data | Mode/codeword interpretation, ECI, FNC1/GS1, structured append |

### 8.2 Input quality tiers

| Tier | Input | Milestone |
|------|-------|-----------|
| 1 | Clean digital images (generated PNGs, rendered PDF pages) | M4 |
| 2 | Scanned documents — rotation, skew, noise, several codes per page | M5 |
| 3 | Camera photos — perspective, blur, glare, uneven light, damage | M6 |

**Honest gap:** tiers 2 and 3 are where mature readers have spent years of tuning. The
algorithms are known; getting high real-world read rates needs a **real image corpus**
(see section 11) and several tuning rounds. This is the largest schedule risk in v1.

### 8.3 Reader options
Formats to look for, effort level (Fast / Normal / Thorough), expect-multiple, try
inverted, try rotated, character-set hint, GS1 mode, cancellation token.

---

## 9. Rendering (`Chuvadi.Barcodes.Rendering`)

| Output | Notes |
|--------|-------|
| SVG | Vector, merged rectangles (one path per run, not one element per module), quiet zone, colours, optional human-readable text for 1D |
| PNG | Exact integer module size (no blurry scaling), 1-bit or 8-bit, DPI metadata |
| Raw | The `BitMatrix` / width pattern itself — for callers drawing their own way |

PDF output comes later through the `Chuvadi.Pdf.Barcodes` adapter in the chuvadi-pdf
repo (vector `re f` path operators), consuming the published packages.

---

## 10. Payloads (`Chuvadi.Barcodes.Payloads`)

Typed builders (object → text) and parsers (text → object). These are formats *carried*
inside a symbol; they are independent of which symbology is used.

| Payload | Build | Parse | Notes |
|---------|:-----:|:-----:|-------|
| URL, plain text | ✅ | ✅ | |
| vCard 3.0/4.0, MeCard | ✅ | ✅ | Contacts |
| Wi-Fi (`WIFI:`) | ✅ | ✅ | |
| SMS, email (`mailto:`), phone (`tel:`) | ✅ | ✅ | |
| Geo (`geo:`), calendar (iCalendar VEVENT) | ✅ | ✅ | |
| UPI (`upi://pay?…`) | ✅ | ✅ | NPCI parameter set |
| EMVCo Merchant-Presented QR (Bharat QR) | ✅ | ✅ | TLV structure + CRC-16 |
| GS1 element strings, GS1 Digital Link | ✅ | ✅ | AI validation, check digits |
| GST e-invoice QR | — | ✅ | Signed JWT issued by the IRP; parse fields, optional signature verification with a caller-supplied certificate |
| Aadhaar Secure QR | — | ✅ | Big-integer → bytes → decompress → fields; optional signature verification with a caller-supplied UIDAI certificate |

All cryptography uses .NET's built-in `System.Security.Cryptography` (no NuGet). We never
generate GST or Aadhaar QR content — only the issuing authorities can sign those.

---

## 11. Testing strategy

| Layer | What it proves |
|-------|---------------|
| Unit tests | Reed–Solomon, BCH, check digits, bit packing, each image format |
| Spec vectors | Worked examples from the standards, compared byte for byte |
| Round-trip | Encode → render → decode for every symbology × size × EC level × mode (property-based where useful) |
| Synthetic distortion | Rotate, skew, perspective-warp, blur, noise, JPEG-compress, damage — then decode |
| Real corpus | Scans and phone photos in `corpus/`, each with an expected-result file; read rate tracked per release |
| Cross-check oracle | *Test-only* comparison against an established reader (e.g. ZXing.Net, Apache-2.0) to catch disagreements — never shipped (open question Q5) |
| Image codecs | Decode reference images and compare pixels against known-good values |

The corpus needs **real images from the field** — wristbands, specimen labels, drug
packs, invoices, phone photos under ward lighting. Only real samples make tier 2/3
tuning meaningful. Anything containing patient data must be synthetic or scrubbed
before it goes into the public repo.

---

## 12. Engineering standards (mirroring Chuvadi PDF)

- .NET 10, `LangVersion latest`, nullable enabled, implicit usings disabled.
- `TreatWarningsAsErrors`, `AnalysisLevel latest`, code style enforced in build.
- XML documentation on every public member; `tools/gen_api_docs.py` output checked in CI.
- Same style rules as Chuvadi (`ThrowIfNull` on public parameters, `?? throw`, braces on
  every control statement, no `var` in `src/`, no unused usings, one initializer
  property per line).
- Central package management; `src/` has zero NuGet dependencies.
- Deterministic builds, embedded PDBs, Source Link.
- CI: style, docs-up-to-date, build & test on ubuntu/windows/macos, pack verify,
  WASM smoke build.

---

## 13. Milestones

Each milestone is one large PR (several features together), with its own design note in
`docs/design/`.

| # | Milestone | Main content | Relative size |
|---|-----------|-------------|---------------|
| M0 | Docs + repository | This proposal, the idea doc, repo creation guide | Small |
| M1 | Scaffold + CI | Solution, props, analyzers, tools, workflows, empty projects, branch protection | Small |
| M2 | Core + encoders + rendering | All of sections 5, 6, 9; spec-vector tests | Large |
| M3 | Imaging | All of section 7 | Large |
| M4 | Decoders, tier 1 | Every symbology decodes clean images; full round-trip suite | Large |
| M5 | Decoders, tier 2 | Scans: rotation, skew, noise, multi-code | Large |
| M6 | Decoders, tier 3 | Camera photos: perspective, blur, glare, damage | Very large |
| M7 | Payloads | All of section 10 | Medium |
| M8 | Hardening → **v1.0.0** | Corpus read-rate targets, fuzzing, benchmarks, docs, CLI polish | Medium |
| M9 | PDF adapter | `Chuvadi.Pdf.Barcodes` in the chuvadi-pdf repo | Medium |

Versions: `0.x.y` during development, mono-versioned; `1.0.0` at the end of M8.

---

## 14. Risks

| Risk | Mitigation |
|------|------------|
| Camera-photo read rates lag mature readers | Real corpus early; measure read rate per build; oracle comparison |
| Standards documents are paid (ISO/IEC, AIM) | Buy the key specs (QR 18004, DataMatrix 16022, PDF417 15438, Aztec 24778, GS1 General Specifications is free) — needed to claim conformance |
| JPEG / TIFF decoders are large | Treat as their own milestone (M3) with reference-image tests |
| Patent or licensing surprises on individual symbologies | Confirm the status of each symbology before v1.0.0; record in the decision log |
| Scope size ("everything") | Milestone-by-milestone PRs; 0.x releases usable before 1.0 |

---

## 15. Open questions

| # | Question | Recommendation |
|---|----------|---------------|
| Q1 | Include the section 3.4 candidates (GS1 Composite, Han Xin, DotCode, legacy stacked, postal) in v1? | GS1 Composite yes (healthcare/retail); rest in v1.x |
| Q2 | WebP / HEIC input? | Exclude from v1; accept raw pixels for those |
| Q3 | Target frameworks — .NET 10 only, or also .NET 8 (LTS)? | .NET 10 only, like Chuvadi PDF |
| Q4 | Ship a `Chuvadi.Barcodes.All` meta-package? | Yes, for convenience |
| Q5 | Use ZXing.Net as a test-only cross-check oracle? | Yes — test projects only, never in `src/` |
| Q6 | Publish to nuget.org at 1.0, or local feeds only (like Chuvadi PDF today)? | Local feeds during 0.x; decide at 1.0 |
| Q7 | Purchase the ISO/IEC specifications? | Yes, before M2 |
