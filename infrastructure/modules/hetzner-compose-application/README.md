# Hetzner Compose Application Module

An OpenTofu module for provisioning a Hetzner Cloud server that automatically deploys and reconciles a Docker Compose application from a Git repository.

## Overview

This module creates a Hetzner Cloud server running Debian 13, installs Docker and Docker Compose, clones a Git repository, and sets up automatic reconciliation that periodically pulls the latest changes and redeploys the application.

## Features

- **Automated Setup**: Server is automatically initialized with Docker and Docker Compose
- **Git Integration**: Clones and tracks a Git repository for your compose application
- **Automatic Reconciliation**: Systemd timer periodically pulls latest changes and redeploys
- **SSH Access**: Configurable admin SSH key for server access

## Usage

### Single Compose File

```hcl
module "my_app" {
  source = "./modules/hetzner-compose-application"

  # Required variables
  application_name      = "my-application"
  git_repository_url    = "https://github.com/user/repo.git"
  git_repository_branch = "main"
  compose_file_paths    = ["compose.yaml"]
  reconciliation_interval = "5m"

  # Optional variables
  admin_ssh_key    = "ssh-ed25519 AAAA..."
  server_type      = "cpx11"
  server_location  = "nbg1"
}
```

### Multiple Compose Files

You can specify multiple compose files to separate platform services from application services:

```hcl
module "my_app" {
  source = "./modules/hetzner-compose-application"

  # Required variables
  application_name      = "my-application"
  git_repository_url    = "https://github.com/user/repo.git"
  git_repository_branch = "main"
  compose_file_paths    = [
    "compose.platform.yaml"
    "compose.yaml"
  ]
  reconciliation_interval = "5m"

  # Optional variables
  admin_ssh_key    = "ssh-ed25519 AAAA..."
  server_type      = "cpx11"
  server_location  = "nbg1"
}
```

## Resources Created

- `hcloud_ssh_key.admin`: SSH key for admin access
- `hcloud_server.server`: Hetzner Cloud server instance

## Variables

### Required

| Name                      | Description                                                                                             | Type           |
|---------------------------|---------------------------------------------------------------------------------------------------------|----------------|
| `application_name`        | Internal name of the application. Used for the server name.                                             | `string`       |
| `git_repository_url`      | URL of the git repository. Only public repositories are supported.                                      | `string`       |
| `git_repository_branch`   | Branch of the git repository                                                                            | `string`       |
| `compose_file_paths`      | List of paths to compose files from the root of the git repository. Multiple files are merged in order. | `list(string)` |
| `reconciliation_interval` | Interval for reconciliation (e.g., "5m", "1h"). The reconciliation pulls latest changes and redeploys   | `string`       |

### Optional

| Name              | Description                                                                                                     | Type     | Default         |
|-------------------|-----------------------------------------------------------------------------------------------------------------|----------|-----------------|
| `admin_ssh_key`   | SSH public key of the admin. A dummy key is used if not provided, and the server is created without SSH access. | `string` | `"(dummy key)"` |
| `server_type`     | Type of the server                                                                                              | `string` | `"cpx11"`       |
| `server_location` | Location of the server                                                                                          | `string` | `"nbg1"`        |

## Outputs

| Name                  | Description                                       |
|-----------------------|---------------------------------------------------|
| `application_address` | Public IP addresses of the server (IPv4 and IPv6) |


## How It Works

### Initialization

On first boot, the server runs the `initialize-compose-host` service which:

1. Installs Docker and Docker Compose from Docker's official repository
2. Clones the specified Git repository to `/opt/app`
3. Marks initialization as complete

### Reconciliation

A systemd timer runs periodically (configurable via `reconciliation_interval`) that:

1. Fetches the latest changes from the Git repository
2. Resets the local repository to match the remote branch
3. Pulls the latest Docker images
4. Redeploys the compose application using `docker compose up` with all specified compose files

Multiple compose files are merged together using Docker Compose's `-f` flag, allowing you to separate platform services from application services for better governance and team contributions.

The reconciliation process is idempotent and will update the deployment whenever changes are detected in the repository.

## Requirements

- OpenTofu >= 1.0
- Hetzner Cloud provider >= 1.58.0
- Valid Hetzner Cloud API token

## Notes

- Only public Git repositories are currently supported
- The reconciliation process uses `docker compose up --no-build`, so it expects pre-built images
- The server image is fixed to `debian-13`
- SSH access requires providing a valid `admin_ssh_key` variable
- **Environment files**: Docker Compose automatically loads `.env` files from the same directory as the compose files. Place `.env` files alongside your compose files in the repository to have them automatically loaded during reconciliation
