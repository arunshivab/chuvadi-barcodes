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
