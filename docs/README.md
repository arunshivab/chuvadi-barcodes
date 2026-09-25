# Chuvadi.Barcodes — Documentation

This folder is the written record of the project, step by step, from idea to release.
Every document is numbered in the order it was written. Nothing here is deleted when it
goes out of date; it is superseded by a later document and the decision log says so.

## Setup

| # | Document | Purpose |
|---|----------|---------|
| 01 | [Create the repository](setup/01-create-repository.md) | Step-by-step instructions to create the GitHub repo, clone it, and land these docs as the first PR |

## Design

| # | Document | Status |
|---|----------|--------|
| 00 | [Project idea](design/00-project-idea.md) | Accepted |
| 01 | [Design proposal](design/01-design-proposal.md) | Draft — for review |
| 02 | [Milestone M1: scaffold and CI](design/02-m1-scaffold.md) | Implemented — in review |
| — | [Decision log](design/DECISIONS.md) | Living document |

Later design notes (one per milestone — encoders, imaging, decoders, payloads) will be
added as `03-…`, `04-…` and so on.

## API reference

Generated from XML doc comments: [api/README.md](api/README.md). Regenerate with
`python3 tools/gen_api_docs.py`.
