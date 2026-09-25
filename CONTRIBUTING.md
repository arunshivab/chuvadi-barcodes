# Contributing to Chuvadi.Barcodes

Thank you for your interest in contributing.

## Before you start

- Check the [open issues](https://github.com/arunshivab/chuvadi-barcodes/issues).
- For large changes, open an issue first to discuss the approach.
- Read the [design proposal](docs/design/01-design-proposal.md) and the
  [decision log](docs/design/DECISIONS.md).

## Development setup

```bash
git clone https://github.com/arunshivab/chuvadi-barcodes.git
cd chuvadi-barcodes
dotnet build Chuvadi.Barcodes.slnx
dotnet test Chuvadi.Barcodes.slnx
```

## Code standards

- C# latest, .NET 10 only.
- Nullable reference types enabled; `TreatWarningsAsErrors` — warnings are fixed, not suppressed.
- XML doc comments on every public member (CS1591 is an error in `src/`).
- `src/` has zero NuGet dependencies. Test projects may use the packages in
  `Directory.Packages.props`.
- One public type per file, file named after the type; file-scoped namespaces.
- `dotnet format --verify-no-changes` must pass.
- `python3 tools/check_style.py <files>` must pass.
- After changing any public API, run `python3 tools/gen_api_docs.py` and commit `docs/api/`.

## Test data

Test images and corpus files must never contain real patient or personal data. Use
synthetic content, or scrub before committing.

## Pull requests

Feature branch → PR → all CI checks green → squash merge. Use the PR template checklist.
