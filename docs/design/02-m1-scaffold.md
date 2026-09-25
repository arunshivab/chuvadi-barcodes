# 02 — Milestone M1: Scaffold and CI

| Field | Value |
|-------|-------|
| Status | Merged (PR #2) |
| Date | 2026-09-25 |
| Depends on | [01 — Design proposal](01-design-proposal.md) |

## 1. What M1 delivers

An empty but fully wired repository: every project from the design proposal exists,
builds with zero warnings, has a smoke test, packs, and runs through the same CI checks
as Chuvadi PDF. No barcode logic yet — that starts in M2.

## 2. Contents

| Path | Purpose |
|------|---------|
| `Chuvadi.Barcodes.slnx` | Solution (src, tests, examples) |
| `Directory.Build.props` | Shared settings, mirrored from Chuvadi PDF |
| `Directory.Packages.props` | Central package versions — test packages only |
| `global.json` | .NET SDK pin (10.0.203, roll forward to latest minor) |
| `.editorconfig` / `.gitattributes` / `.gitignore` | Same rules as Chuvadi PDF |
| `src/Chuvadi.Barcodes` | Core (no project references) |
| `src/Chuvadi.Barcodes.Encoders` | → Core |
| `src/Chuvadi.Barcodes.Decoders` | → Core |
| `src/Chuvadi.Barcodes.Imaging` | No project references |
| `src/Chuvadi.Barcodes.Rendering` | → Core, Imaging |
| `src/Chuvadi.Barcodes.Payloads` | → Core |
| `tests/<project>.Tests` (×6) | One per src project; M1 has a load smoke test in each |
| `tests/Chuvadi.Barcodes.Conformance.Tests` | Cross-project round-trip and corpus tests (from M4) |
| `tests/Chuvadi.Barcodes.WasmSmoke` | WebAssembly build check; not in the solution, built by CI |
| `examples/Chuvadi.Barcodes.Cli` | `chuvadi-barcodes` command-line tool; prints its version in M1 |
| `tools/check_style.py` | Ported style checker, plus new Rule 5 (see §4) |
| `tools/gen_api_docs.py` | Ported API doc generator; modules are Core, Encoders, Decoders, Imaging, Rendering, Payloads |
| `build/pack.ps1` | Mono-versioned pack of all six packages |
| `.github/workflows/ci.yml`, `build.yml` | CI (§3) |
| `.github/pull_request_template.md` | PR checklist |
| `corpus/README.md` | Rules for the decoder test corpus (no patient data) |
| `README.md`, `CHANGELOG.md`, `CONTRIBUTING.md`, `CLAUDE.md` | Repository documents |
| `docs/api/README.md` | Generated API index (empty until M2) |

## 3. CI checks

Same workflows and job names as Chuvadi PDF, so the check names are identical (D-009):

| Workflow | Check name(s) | What it does |
|----------|---------------|-------------|
| `build.yml` | `style` | Runs `tools/check_style.py` on all `.cs` files; rejects stray delivery artifacts |
| `build.yml` | `docs-up-to-date` | Regenerates `docs/api/` and fails if it differs from the commit |
| `build.yml` | `build-ubuntu-latest`, `build-windows-latest`, `build-macos-latest` | Restore, build (Release), test |
| `ci.yml` | `Build & Test (ubuntu-latest)`, `Build & Test (windows-latest)`, `Build & Test (macos-latest)` | Build and unit tests |
| `ci.yml` | `Code Style` | `dotnet format --verify-no-changes` |
| `ci.yml` | `Pack Verify` | `dotnet pack` of all packages |
| `ci.yml` | `WASM Smoke` | Installs `wasm-tools`, builds the WASM smoke project |

## 4. Changes from the design proposal

**Project rename (D-010).** The proposal named two projects `Chuvadi.Barcodes.Encoding`
and `Chuvadi.Barcodes.Decoding`. During M1 this was proven to break the build: a
namespace `Chuvadi.Barcodes.Encoding` hides `System.Text.Encoding` in every other
`Chuvadi.Barcodes.*` namespace that references it — `Encoding.UTF8` then fails with
CS0234. They are now **`Chuvadi.Barcodes.Encoders`** and **`Chuvadi.Barcodes.Decoders`**.
`tools/check_style.py` gained Rule 5, which rejects namespace segments named after
common System types so this cannot recur.

**XML docs enforced from day one (D-011).** Chuvadi PDF suppresses CS1591 during
development; here it is an error in `src/` from the start. Test and example projects
opt out.

**No package icon yet (D-012).** `icon.png` is omitted until a Chuvadi.Barcodes logo
exists.

## 5. Verified in Claude's environment before delivery

- Clean build of `Chuvadi.Barcodes.slnx` in Release: 0 warnings, 0 errors.
- `dotnet test`: 12 tests, all passing.
- `dotnet format --verify-no-changes`: clean.
- `tools/check_style.py` on all `.cs` files: passed.
- `tools/gen_api_docs.py`: `docs/api/` unchanged after regeneration.
- `dotnet pack`: six packages produced.
- WASM smoke project builds with the `wasm-tools` workload.
- CLI runs and prints its version.
