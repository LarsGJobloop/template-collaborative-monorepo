#!/usr/bin/env bash
set -euo pipefail

# Set noninteractive mode
export DEBIAN_FRONTEND=noninteractive
export DEBCONF_NONINTERACTIVE_SEEN=true

# Validate branch name
if ! [[ "${git_repository_branch}" =~ ^[a-zA-Z0-9._/-]+$ ]]; then
  echo "ERROR: Invalid branch name: ${git_repository_branch}"
  exit 1
fi

# Install prerequisites
apt-get update -y

# Add Docker GPG key
install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/debian/gpg -o /etc/apt/keyrings/docker.asc
chmod a+r /etc/apt/keyrings/docker.asc

# Add Docker repository
cat >/etc/apt/sources.list.d/docker.sources <<EOF
Types: deb
URIs: https://download.docker.com/linux/debian
Suites: $(. /etc/os-release && echo "$VERSION_CODENAME")
Components: stable
Signed-By: /etc/apt/keyrings/docker.asc
EOF

# Update package list
apt-get update -y
# Install Docker and Compose
apt-get install -y \
  docker-ce \
  docker-ce-cli \
  containerd.io \
  docker-buildx-plugin \
  docker-compose-plugin

# Enable and start Docker
systemctl enable --now docker

# Create application directory
install -m 0755 -d /opt/app

# Clone repository
if [ ! -d /opt/app/.git ]; then
  git clone --branch "${git_repository_branch}" \
    --single-branch \
    "${git_repository_url}" \
    /opt/app

  # Configure git repository security settings
  cd /opt/app
  git config core.hooksPath /dev/null
  git config remote.origin.fetch "+refs/heads/${git_repository_branch}:refs/remotes/origin/${git_repository_branch}"
fi

# Mark initialization complete
install -m 0755 -d /var/lib
touch /var/lib/compose-host.initialized
