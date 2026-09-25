# Changelog

All notable changes to Chuvadi.Barcodes are recorded here. Versions follow SemVer; all
packages share one version (mono-versioning).

## [Unreleased]

### Added
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
