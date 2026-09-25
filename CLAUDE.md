# CLAUDE.md — Chuvadi.Barcodes Working Agreement

> Read this at the start of every session.
> Design: docs/design/01-design-proposal.md · Decisions: docs/design/DECISIONS.md

## 1. Project identity

**Chuvadi.Barcodes** — part of the Chuvadi (சுவடி) family.
Pure managed .NET 10 barcode and QR code library: encode and decode every major open
1D/2D symbology. Zero production dependencies. Apache-2.0.

```
PROJECT:   Chuvadi.Barcodes
STACK:     C# (.NET 10, LangVersion latest)
SOLUTION:  Chuvadi.Barcodes.slnx (6 src + 7 test + 1 example; WasmSmoke built separately)
DEPS:      Zero in src/. xUnit + FluentAssertions + FsCheck in tests/.
REPO:      github.com/arunshivab/chuvadi-barcodes (public)
LOCAL:     C:\Users\aruns\Documents\Chuvadi\chuvadi-barcodes\
BUILD:     dotnet build Chuvadi.Barcodes.slnx
TEST:      dotnet test Chuvadi.Barcodes.slnx
STYLE:     python3 tools/check_style.py <files>
API DOCS:  python3 tools/gen_api_docs.py
PACK:      .\build\pack.ps1 -Version x.y.z
```

## 2. Workflow

1. Clone (or reuse) the repo; `git reset --hard origin/main`. A fresh clone of
   origin/main is the authoritative source.
2. Make the change; clean bin/obj → build → test → format → API docs → style, until green.
3. Deliver **complete files only** (UTF-8 no BOM, LF) as a zip — never snippets or diffs.
4. Provide PowerShell to extract → copy into the repo → delete stale files → clean
   bin/obj → build → test → run.
5. Provide git commands (feature branch → PR → CI → squash merge) and cleanup commands.
   Hold all git operations until Arun confirms. Arun does visual verification.

## 3. Rules that break the build if violated

- CA1062: `ArgumentNullException.ThrowIfNull` on every public method parameter.
- IDE0270: `?? throw`, not `if (x == null) throw`.
- IDE0005: no unused usings (src and tests). Parent namespaces are implicit.
- IDE0008: no `var` in `src/`.
- IDE0011: braces on every control-flow statement.
- One property per line in object initializers.
- CS1591: XML docs on every public member in `src/`.
- Never name a namespace segment after a common System type (`Encoding`, `Path`,
  `Stream`, …) — it hides the System type in sibling namespaces (see D-010).
- Tests: no `.ConfigureAwait(false)` in `[Fact]` bodies (xUnit1030);
  `Assert.Contains(item, collection)` (xUnit2017); hoist `new[] {…}` to a static
  readonly field (CA1861).

## 4. Never guess

Do not guess at APIs, types, or signatures of any external code. Confirm from source
or ask. For standards (ISO/IEC, AIM, GS1), cite the clause or test vector used.

## 5. Documentation

Every step is documented in `docs/`. Every decision gets a D-entry in
`docs/design/DECISIONS.md`. Each milestone gets a design note in `docs/design/`.
