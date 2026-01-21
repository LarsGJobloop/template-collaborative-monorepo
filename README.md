<div align="center">
  <a href="https://larsgunnar.no">
    <img alt="Logo" src="./assets/logo/logo.svg" height="128">
  </a>
  <h1>Collaborative Monorepo Template</h1>

  <div>
    <a href="https://larsgunnar.no"><img alt="Logo" src="https://img.shields.io/badge/MADE_BY_LG-441306?style=for-the-badge"></a>
    <a href="https://github.com/LarsGJobloop/template-collaborative-monorepo/blob/main/LICENSE"><img alt="License" src="https://img.shields.io/github/license/LarsGJobloop/template-collaborative-monorepo?style=for-the-badge&labelColor=441306&color=441306"></a>
    <a href="https://opentofu.org/"><img alt="OpenTofu" src="https://img.shields.io/badge/OpenTofu-441306?style=for-the-badge&logo=opentofu"></a>
    <a href="https://nixos.org/"><img alt="Nix" src="https://img.shields.io/badge/Nix_Flake-441306?style=for-the-badge&logo=nixos"></a>
  </div>

  <div>
    <a href="https://github.com/LarsGJobloop/template-collaborative-monorepo/actions/workflows/publish-oci-manifests.yml"><img alt="OCI Manifests" src="https://img.shields.io/github/actions/workflow/status/LarsGJobloop/template-collaborative-monorepo/publish-oci-manifests.yml?branch=main&style=for-the-badge&label=OCI%20Manifests&labelColor=441306"></a>
    <a href="https://github.com/LarsGJobloop/template-collaborative-monorepo"><img alt="Repository Status" src="https://img.shields.io/github/last-commit/LarsGJobloop/template-collaborative-monorepo?style=for-the-badge&label=Last%20Updated&labelColor=441306"></a>
  </div>
</div>


A template for managing multiple applications and services in a single repository with infrastructure as code, CI/CD, and deployment automation.

## Structure

- **`src/`** - Source code for applications and services
- **`infrastructure/`** - Terraform/OpenTofu configurations
  - **`environments/`** - Environment-specific deployments (development, production)
  - **`modules/`** - Reusable infrastructure modules
- **`project/`** - Project-level Terraform (GitHub repository management)
- **`templates/`** - Project templates for scaffolding new services
- **`.github/workflows/`** - CI/CD workflows

## Quick Start

### Development

```sh
# Enter development shell
nix develop

# Start local services
docker compose up
```

Services available at:
- Example API: http://localhost:6000
- Example Next.js UI: http://localhost:6001
- Example Vanilla UI: http://localhost:6002

### Infrastructure

```sh
# Set up GitHub repository
cd project && terraform init && terraform apply

# Deploy to development
cd infrastructure/environments/development && terraform init && terraform apply
```

## CI/CD

The `publish-oci-manifests.yml` workflow automatically:
- Discovers services with Dockerfiles in `src/`
- Builds multi-platform images (`linux/amd64`, `linux/arm64`)
- Publishes to GitHub Container Registry: `ghcr.io/<owner>/<service-name>`
- Tags: `latest`, branch name, and commit SHA

## Example Services

- **`example-web-api-service`** - ASP.NET Core API
- **`example-web-ui-nextjs`** - Next.js application
- **`example-web-ui-vanilla`** - Vanilla JavaScript with ES modules

## Infrastructure Modules

**`hetzner-compose-application`** - Terraform module for deploying Docker Compose applications to Hetzner Cloud with automated Git-based reconciliation.

## Documentation

- [`src/README.md`](src/README.md) - Source code organization
- [`infrastructure/README.md`](infrastructure/README.md) - Infrastructure as Code
- [`project/README.md`](project/README.md) - Project governance
- [`.github/README.md`](.github/README.md) - GitHub Actions workflows
