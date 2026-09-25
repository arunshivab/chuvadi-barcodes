## Summary

<!-- One-paragraph description of what this PR changes. -->


## Motivation

<!-- Why is this change needed? Link to the milestone design note or issue. -->


## Type of change

- [ ] Feature (new symbology, format, payload or capability)
- [ ] Bug fix
- [ ] Refactor (no behavioural change)
- [ ] Documentation only
- [ ] Build/CI/tooling

## Testing

- [ ] `dotnet build` passes with zero warnings
- [ ] `dotnet test` passes (all tests green)
- [ ] `dotnet format --verify-no-changes` passes
- [ ] `python3 tools/check_style.py <changed-files>` passes
- [ ] New tests added for new behaviour (spec vectors / round-trip / corpus as applicable)
- [ ] No patient or personal data in any test image or corpus file

## Documentation

- [ ] `python3 tools/gen_api_docs.py` run and `docs/api/` committed
- [ ] `docs/design/DECISIONS.md` — new D-entry for every decision taken
- [ ] Milestone design note in `docs/design/` updated
- [ ] `CHANGELOG.md` updated
- [ ] `README.md` — capability table updated if user-visible

## Self-review checklist

- [ ] No `var` in `src/` (IDE0008)
- [ ] Braces on all control flow (IDE0011)
- [ ] All public params validated against null (CA1062)
- [ ] XML docs on every public member (CS1591 is an error in `src/`)
- [ ] No namespace segment that shadows a System type (e.g. `Encoding`, `Path`, `Stream`)
- [ ] Test csproj has no `Version=` attributes (central package management)
- [ ] `src/` has zero NuGet dependencies

## Related

<!-- Closes #N, related to #M, etc. -->
