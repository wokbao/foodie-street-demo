#!/usr/bin/env bash
set -euo pipefail

# Quick helper to commit & push a submodule and bump the parent repo pointer.
# Usage:
#   scripts/submodule_commit.sh <submodule-path> <submodule-commit-message> [parent-commit-message]

if [[ $# -lt 2 ]]; then
  echo "Usage: $0 <submodule-path> <submodule-commit-message> [parent-commit-message]" >&2
  exit 1
fi

SUBMODULE_PATH="$1"
SUBMODULE_MSG="$2"
PARENT_MSG="${3:-chore: bump $(basename "$SUBMODULE_PATH") submodule}"

if [[ ! -d "$SUBMODULE_PATH/.git" ]]; then
  echo "Error: $SUBMODULE_PATH is not a valid submodule path (missing .git directory)." >&2
  exit 1
fi

echo ">>> Committing submodule: $SUBMODULE_PATH"
git -C "$SUBMODULE_PATH" status -sb
git -C "$SUBMODULE_PATH" add -A
git -C "$SUBMODULE_PATH" commit -m "$SUBMODULE_MSG"
git -C "$SUBMODULE_PATH" push

echo ">>> Bumping parent pointer"
git add "$SUBMODULE_PATH"
if git diff --cached --quiet; then
  echo "No parent changes to commit."
else
  git commit -m "$PARENT_MSG"
  git push
fi

echo "Done."
