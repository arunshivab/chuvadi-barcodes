# 03 — Milestone M2a: Core, QR Family Encoders, Rendering

| Field | Value |
|-------|-------|
| Status | Implemented — in review |
| Date | 2026-09-26 |
| Depends on | [01 — Design proposal](01-design-proposal.md), [02 — M1 scaffold](02-m1-scaffold.md) |

## 1. Scope

M2 (core + all encoders + rendering) is split into four PRs (D-015):

| PR | Content | State |
|----|---------|-------|
| **M2a** | Core types, QR Code Model 2, Micro QR, structured append, SVG/PNG rendering, CLI `encode` | **This note** |
| M2b | rMQR, Data Matrix, Aztec, PDF417, MicroPDF417, MaxiCode | Next |
| M2c | All 1D and postal symbologies | |
| M2d | Han Xin, DotCode, Codablock F, Code 16K, Code 49, GS1 Composite | |

rMQR moved from M2a to M2b (D-017): its size and block tables must be sourced first.

## 2. Public API added

| Package | Types |
|---------|-------|
| `Chuvadi.Barcodes` | `BarcodeFormat` (all 42 formats), `BitMatrix`, `LuminanceImage`, `QrErrorCorrectionLevel` |
| `Chuvadi.Barcodes.Encoders` | `QrEncoder`, `MicroQrEncoder`, `QrEncodeOptions`, `MicroQrEncodeOptions`, `QrCharacterSet`, `QrSymbol`, `BarcodeEncodingException` |
| `Chuvadi.Barcodes.Imaging` | `PngWriter` (8-bit greyscale; readers come in M3) |
| `Chuvadi.Barcodes.Rendering` | `SvgRenderer`, `SvgRenderOptions`, `RasterRenderer`, `PngRenderer`, `RasterRenderOptions` |

Generated reference: [`docs/api/`](../api/README.md).

```csharp
QrSymbol qr = QrEncoder.Encode("https://example.org");
string svg = SvgRenderer.Render(qr.Matrix, new SvgRenderOptions { QuietZone = qr.QuietZone });
byte[] png = PngRenderer.Render(qr.Matrix, new RasterRenderOptions { ModuleSize = 8, QuietZone = qr.QuietZone });
```

## 3. What the QR encoder does

| Feature | Detail |
|---------|--------|
| Versions | 1–40, smallest that fits within `MinVersion`–`MaxVersion` |
| Levels | L, M, Q, H; optional boost to the highest level that keeps the version |
| Modes | Numeric, alphanumeric, byte, Kanji — **optimal segmentation** by dynamic programming (costs in sixths of a bit) |
| Character sets | Auto (ISO-8859-1 if possible, else UTF-8 + ECI 26), Latin-1, UTF-8, Shift JIS (Kanji mode) |
| GS1 | FNC1 in first position; `U+001D` becomes `%` in alphanumeric mode, literal `%` becomes `%%` |
| Structured append | 2–16 symbols, parity = XOR of all data bytes; `SplitForStructuredAppend` exposes the split |
| Masks | Fixed 0–7 or automatic by the four ISO penalty rules (see §5) |
| Micro QR | M1–M4, levels L/M/Q as allowed per version, 4 masks with the Micro QR edge evaluation, Latin-1/UTF-8/Shift JIS |

Shift JIS uses `CodePagesEncodingProvider` from the .NET base library (no NuGet package).

## 4. How correctness is proven without the ISO specification (D-014, D-016)

| Check | What it proves |
|-------|---------------|
| **Zint golden fixtures** — 131 cases in `tests/Chuvadi.Barcodes.Encoders.Tests/Fixtures/Zint/` | Our symbol is identical, module for module, to Zint 2.13.0's for: every version 1–40 (one case each, cycling levels and masks), numeric/alphanumeric/byte/mixed, Latin-1, UTF-8 + ECI, Kanji, GS1, structured append, 32 QR and 8 Micro QR automatic-mask cases, and all 8 Micro QR version/level combinations × 4 masks |
| **ZXing.Net round trips** — 160 version × level cases plus features | An independent reader decodes every symbol back to the original text and reports the same level and structured-append sequence |
| **Published worked example** | Reed–Solomon ECC for "HELLO WORLD" 1-M equals the widely published codewords |
| **Mutation check** | Flipping one module in a fixture makes its test fail (verified manually) |

Zint and ZXing.Net are **test oracles only**: `src/` references neither, the NuGet packages
depend on nothing but each other, and no code or tables were copied from either (D-016).
Fixtures are regenerated with `python3 tools/gen_zint_fixtures.py` (needs `zint` on PATH;
CI does not).

## 5. Automatic mask selection

The first implementation scored penalty rule 3 in the style of a popular open-source
encoder and chose a different (still valid) mask from Zint in 10 of 44 samples; data bits
were identical in all 44. Testing interpretations against Zint showed that this reading of
ISO/IEC 18004 reproduces Zint in 44 of 44 (D-018):

- Rule 1: each run of ≥ 5 same-colour modules in a row or column scores 3 + (run − 5).
- Rule 2: each 2 × 2 block of one colour scores 3.
- Rule 3: each `1011101` pattern preceded **or** followed by four light modules scores 40
  once; modules outside the symbol count as light.
- Rule 4: 10 × (⌈|dark% − 50| / 5⌉ − 1).

It is now covered by the automatic-mask fixtures.

## 6. Rendering

- **SVG**: view box in modules, one `<path>` with horizontal runs merged, optional
  transparent background, `shape-rendering="crispEdges"`.
- **Raster/PNG**: exact integer pixels per module, quiet zone in modules, optional DPI
  (`pHYs`). PNG is written by our own `PngWriter` (zlib from the base library, own CRC-32).

## 7. CLI

```
chuvadi-barcodes encode --format qr|microqr --data TEXT --out FILE.svg|FILE.png
    [--ecc L|M|Q|H] [--version N] [--mask N] [--charset auto|latin1|utf8|shiftjis]
    [--no-eci] [--gs1] [--boost] [--structured-append N] [--scale N] [--quiet N] [--dpi N]
```

## 8. Test totals

346 tests: 315 encoder (131 Zint fixtures, 184 round-trip and behaviour), 14 core,
5 rendering, 4 imaging, 8 smoke.
