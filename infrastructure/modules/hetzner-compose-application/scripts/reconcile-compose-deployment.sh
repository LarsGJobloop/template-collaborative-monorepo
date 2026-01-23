#!/usr/bin/env bash
set -euo pipefail

echo "Reconciliation started"

cd /opt/app

git config core.hooksPath /dev/null

old_revision="$(git rev-parse HEAD)"
echo "Old revision: $old_revision"

git fetch origin "refs/heads/${git_repository_branch}:refs/remotes/origin/${git_repository_branch}"

new_revision="$(git rev-parse "origin/${git_repository_branch}")"
echo "New revision: $new_revision"

git reset --hard "origin/${git_repository_branch}"

# Build array of compose file flags
compose_file_flags=()
for compose_file_path in ${compose_file_paths}; do
  # Skip empty entries
  [ -z "$compose_file_path" ] && continue
  
  # Resolve and validate compose file path
  compose_full_path="$(realpath -m "/opt/app/$compose_file_path")"
  if [[ "$compose_full_path" != /opt/app/* ]]; then
    echo "ERROR: Invalid compose_file_path: $compose_file_path"
    exit 1
  fi
  
  # Check if there's a compose file at the specified path
  if [ ! -f "$compose_full_path" ]; then
    echo "ERROR: Compose file not found: $compose_full_path"
    exit 1
  fi
  
  compose_file_flags+=("--file" "$compose_full_path")
done

if [ $${#compose_file_flags[@]} -eq 0 ]; then
  echo "ERROR: No compose files specified"
  exit 1
fi

echo "Pulling latest images"
docker compose "$${compose_file_flags[@]}" pull

echo "Redeploying services"
docker compose "$${compose_file_flags[@]}" up -d --no-build --remove-orphans

echo "Reconciliation completed"
