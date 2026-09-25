# 01 — Create the Repository

| Field | Value |
|-------|-------|
| Status | Ready to follow (Step 6 updated in M1 with the exact check names) |
| Date | 2026-09-25 |
| Result | Public repo `github.com/arunshivab/chuvadi-barcodes`, cloned locally, with these docs merged as the first PR |

Follow the steps in order. Each step ends with a **Check** — do not move on until it passes.

---

## Step 0 — Prerequisites (PowerShell)

```powershell
git --version        # any recent version
dotnet --version     # should be 10.0.x
```

**Check:** both commands print a version. You are signed in to GitHub in the browser as
`arunshivab`.

---

## Step 1 — Create the repository on GitHub

1. Open <https://github.com/new>.
2. Fill in:

   | Field | Value |
   |-------|-------|
   | Owner | `arunshivab` |
   | Repository name | `chuvadi-barcodes` |
   | Description | `Pure .NET 10 barcode & QR code library — encode and decode every major 1D/2D symbology. Zero dependencies. Part of the Chuvadi family.` |
   | Visibility | **Public** |
   | Add a README file | **On** (so the repo has a `main` branch to clone) |
   | Add .gitignore | **None** (we supply our own in milestone M1) |
   | Choose a license | **Apache License 2.0** |

3. Click **Create repository**.

**Check:** the repo page shows `README.md` and `LICENSE`, and the branch selector says `main`.

---

## Step 2 — Repository settings

Open the repo → **Settings**.

### 2a. General → Features
- Issues: **On**
- Wikis: **Off** (docs live in `docs/`)
- Projects: Off (optional)
- Discussions: Off for now (optional)

### 2b. General → Pull Requests
- Allow merge commits: **Off**
- Allow squash merging: **On** — default message: *Pull request title*
- Allow rebase merging: **Off**
- Always suggest updating pull request branches: **On**
- Automatically delete head branches: **On**

### 2c. About (gear icon on the repo's main page, right side)
- Topics: `dotnet`, `csharp`, `barcode`, `qrcode`, `datamatrix`, `pdf417`, `aztec`, `gs1`, `barcode-reader`, `barcode-generator`

**Check:** settings saved; topics visible on the repo page.

> Branch protection is **not** set up yet. It needs the CI check names, which only exist
> after milestone M1. It is Step 6 below.

---

## Step 3 — Clone locally

```powershell
Set-Location C:\Users\aruns\Documents\Chuvadi
git clone https://github.com/arunshivab/chuvadi-barcodes.git
Set-Location .\chuvadi-barcodes
git status
```

**Check:** `git status` says `On branch main … nothing to commit, working tree clean`.

Local repo path from now on: `C:\Users\aruns\Documents\Chuvadi\chuvadi-barcodes\`

---

## Step 4 — Add these docs (first PR)

The docs arrive as `chuvadi-barcodes-docs-m0.zip` in your Downloads folder. Extract it
**outside** the repo, then copy.

```powershell
$zip  = "$env:USERPROFILE\Downloads\chuvadi-barcodes-docs-m0.zip"
$tmp  = "$env:USERPROFILE\Downloads\chuvadi-barcodes-docs-m0"
$repo = "C:\Users\aruns\Documents\Chuvadi\chuvadi-barcodes"

if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
Expand-Archive -Path $zip -DestinationPath $tmp
Copy-Item -Path "$tmp\docs" -Destination $repo -Recurse -Force

Set-Location $repo
Get-ChildItem -Recurse .\docs | Select-Object FullName
```

**Check:** you see `docs\README.md`, `docs\setup\01-create-repository.md`,
`docs\design\00-project-idea.md`, `docs\design\01-design-proposal.md`,
`docs\design\DECISIONS.md`.

Now commit on a feature branch and push:

```powershell
git checkout -b docs/m0-project-proposal
git add docs
git status
git commit -m "docs: project idea, design proposal, decision log, repo setup guide (M0)"
git push -u origin docs/m0-project-proposal
```

**Check:** the push prints a link to create a pull request.

---

## Step 5 — Open and merge the PR

1. Open the link from the push (or the repo page → **Compare & pull request**).
2. Title: `docs: project idea, design proposal, decision log, repo setup guide (M0)`
3. Click **Create pull request**. (No CI checks exist yet — that is expected.)
4. Review the rendered Markdown in the **Files changed** tab.
5. Click **Squash and merge** → **Confirm**.

Then clean up locally:

```powershell
git checkout main
git pull
git branch -d docs/m0-project-proposal
git remote prune origin
Remove-Item "$env:USERPROFILE\Downloads\chuvadi-barcodes-docs-m0" -Recurse -Force
```

**Check:** `docs/` is visible on github.com on `main`; `git branch` shows only `main`.

---

## Step 6 — Branch protection (after the M1 PR has run CI once)

GitHub only offers check names it has already seen reported. Do this step **after** the
M1 scaffold PR has run its checks at least once (they appear on the PR page). If the
search box shows only "Add <what you typed> — Any source", the check has not run yet —
do not add it: a required check that never reports blocks every merge.

1. Settings → **Branches** → **Add branch ruleset** (or *Add classic branch protection
   rule*).
2. Ruleset name: `main-protection`. Enforcement status: **Active**.
   Target branches → **Add target** → **Include default branch**.
3. Enable:
   - **Restrict deletions**
   - **Require a pull request before merging** (required approvals: 0 — single maintainer)
   - **Require status checks to pass** → tick **Require branches to be up to date before
     merging** → **Add checks** and add each of these (search by the name; pick the entry
     that shows the GitHub Actions source):

     | Check | From workflow |
     |-------|---------------|
     | `style` | build.yml |
     | `docs-up-to-date` | build.yml |
     | `build-ubuntu-latest` | build.yml |
     | `build-windows-latest` | build.yml |
     | `build-macos-latest` | build.yml |
     | `Build & Test (ubuntu-latest)` | ci.yml |
     | `Build & Test (windows-latest)` | ci.yml |
     | `Build & Test (macos-latest)` | ci.yml |
     | `Code Style` | ci.yml |
     | `Pack Verify` | ci.yml |
     | `WASM Smoke` | ci.yml |

   - **Block force pushes**
4. **Create** / **Save changes**.

**Check:** the M1 PR page now lists these as *Required*. After merge, a direct
`git push origin main` of a throwaway local commit must be rejected; then run
`git reset --hard origin/main` to drop that commit.

---

## Step 7 — Tell Claude

Say "repo is ready". From then on, Claude clones
`https://github.com/arunshivab/chuvadi-barcodes` directly (it is public) as the
authoritative source, and milestone M1 (scaffold + CI) begins.
