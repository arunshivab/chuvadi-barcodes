# Decision Log — Chuvadi.Barcodes

Every decision is recorded here with its date and source. Decisions are never edited
after the fact; a later decision supersedes an earlier one by number.

| # | Date | Decision | Source |
|---|------|----------|--------|
| D-001 | 2026-09-25 | Build a QR/barcode library with both encoder and decoder. | Arun |
| D-002 | 2026-09-25 | Standalone library for everyone, in the Chuvadi family, in its own public repo `arunshivab/chuvadi-barcodes`. | Arun |
| D-003 | 2026-09-25 | Everything in v1: all open symbologies in design proposal §3.1–3.3. | Arun |
| D-004 | 2026-09-25 | Decoder must handle clean images, scans and camera photos. | Arun |
| D-005 | 2026-09-25 | Write our own image decoders; no external imaging dependency. | Arun |
| D-006 | 2026-09-25 | Open source for everyone. License: Apache-2.0, matching Chuvadi PDF. | Arun (license choice to confirm) |
| D-007 | 2026-09-25 | Exclude proprietary symbologies (Denso iQR, SQRC, Frame QR). | Arun |
| D-008 | 2026-09-25 | Documentation lives in the repo under `docs/` (`docs/design/`, `docs/setup/`). | Arun |
| D-009 | 2026-09-25 | CI workflows and check names are the same as Chuvadi PDF (`style`, `docs-up-to-date`, `build-<os>`, `Build & Test (<os>)`, `Code Style`, `Pack Verify`, `WASM Smoke`). | Arun |
| D-010 | 2026-09-25 | Projects renamed from `.Encoding` / `.Decoding` to `Chuvadi.Barcodes.Encoders` / `Chuvadi.Barcodes.Decoders`: a namespace segment `Encoding` hides `System.Text.Encoding` (CS0234, reproduced during M1). Style checker Rule 5 now forbids such segments. Supersedes the project names in design proposal §4. | Claude (M1 build finding) |
| D-011 | 2026-09-25 | XML docs mandatory in `src/` from day one (CS1591 not suppressed). | Claude (M1) |
| D-012 | 2026-09-25 | No package icon until a Chuvadi.Barcodes logo exists. | Claude (M1) |
