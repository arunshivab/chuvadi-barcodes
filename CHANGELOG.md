# Changelog

All notable changes to Chuvadi.Barcodes are recorded here. Versions follow SemVer; all
packages share one version (mono-versioning).

## [Unreleased]

### Added — M2a
- Core: `BarcodeFormat` (all 42 planned formats), `BitMatrix`, `LuminanceImage`,
  `QrErrorCorrectionLevel`; internal `BitBuffer`, `GaloisField`, `ReedSolomonEncoder`.
- Encoders: `QrEncoder` (QR Code Model 2, versions 1–40, optimal mode segmentation,
  Latin-1 / UTF-8 + ECI 26 / Shift JIS Kanji, GS1 FNC1, structured append, error-correction
  boost, automatic mask selection matching Zint) and `MicroQrEncoder` (M1–M4).
- Rendering: `SvgRenderer`, `RasterRenderer`, `PngRenderer`; Imaging: `PngWriter`.
- CLI: `encode` command for QR and Micro QR to SVG or PNG.
- Tests: 131 Zint golden fixtures (module-for-module), ZXing.Net round trips for every
  QR version × level; `tools/gen_zint_fixtures.py`. Zint and ZXing.Net are test-only.
- Docs: M2a design note, decisions D-013 to D-021.

### Added — M1
- M1 scaffold: solution `Chuvadi.Barcodes.slnx`, six library projects
  (`Chuvadi.Barcodes`, `.Encoders`, `.Decoders`, `.Imaging`, `.Rendering`, `.Payloads`),
  one test project each plus `Chuvadi.Barcodes.Conformance.Tests`, the
  `chuvadi-barcodes` CLI example, and a WebAssembly smoke project.
- Build settings mirroring Chuvadi PDF (`Directory.Build.props`, `.editorconfig`,
  central package management); XML docs mandatory in `src/` from day one.
- CI workflows `ci.yml` and `build.yml` with the same check names as Chuvadi PDF.
- Tools: `tools/check_style.py` (with a new rule against namespace segments that
  shadow System types) and `tools/gen_api_docs.py`; `build/pack.ps1`.
- Documentation: M0 idea and design proposal, decision log, repository setup guide,
  M1 scaffold note.
