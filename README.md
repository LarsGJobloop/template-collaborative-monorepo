<div align="center">
  <a href="https://platform.sandefjord.kodehode.larsgunnar.no">
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
    <a href="https://platform.sandefjord.kodehode.larsgunnar.no"><img alt="Website" src="https://img.shields.io/website?url=https%3A%2F%2Fplatform.sandefjord.kodehode.larsgunnar.no&up_message=UP&up_color=7ccf00&down_message=DOWN&down_color=e7000b&style=for-the-badge&logo=docker&logoColor=e60076&label=Platform&labelColor=441306"></a>
    <a href="https://github.com/LarsGJobloop/template-collaborative-monorepo/actions/workflows/publish-oci-manifests.yml"><img alt="OCI Manifests" src="https://img.shields.io/github/actions/workflow/status/LarsGJobloop/template-collaborative-monorepo/publish-oci-manifests.yml?branch=main&style=for-the-badge&label=OCI%20Manifests&labelColor=441306"></a>
    <a href="https://github.com/LarsGJobloop/template-collaborative-monorepo"><img alt="Repository Status" src="https://img.shields.io/github/last-commit/LarsGJobloop/template-collaborative-monorepo?style=for-the-badge&label=Last%20Updated&labelColor=441306"></a>
  </div>
</div>

A template for managing multiple applications and services in a single repository with infrastructure as code, CI/CD, and deployment automation.

## Repository Structure

```
.
├── compose.platform.yaml          # Platform services for local development
├── compose.yaml                   # Application services for local development
├── src/                           # Independent services/applications
│   ├── example-web-api-service/
│   ├── example-web-ui-nextjs/
│   └── ...n                       # Additional services
├── infrastructure/                # Platform configurations (reconciled from repo)
│   ├── environments/
│   │   ├── development/
│   │   │   ├── compose.platform.yaml
│   │   │   ├── compose.yaml
│   │   │   └── *.tf
│   │   └── ...n                   # Additional environments
│   └── modules/
│       ├── hetzner-compose-application/
│       └── ...n                   # Additional modules
├── project/                       # Project governance and dependencies
│   └── *.tf
├── .github/workflows/             # CI/CD flows
│   └── publish-oci-manifests.yml
└── templates/                    # Ready-made service templates
```

**Key Points:**

- **Root compose files** (`compose.platform.yaml`, `compose.yaml`) - For local development
- **Environment compose files** (`infrastructure/environments/*/compose.*.yaml`) - Define what gets reconciled in deployed environments
- **Separation:** Platform services and application services are in separate compose files

## Prerequisites

For infrastructure provisioning and deployment, you'll need:

- **Hetzner Cloud Account** - For hosting servers ([Sign up](https://www.hetzner.com/cloud))
- **deSEC Account** - For DNS management ([Sign up](https://desec.io/))
- **Registered Domain Name** - Domain that will be managed by deSEC

## Quick Start

### Local Development

> [!NOTE]
> No external accounts required! Local development works independently - you can run platform services locally without Hetzner Cloud, deSEC, or domain registration.

For app developers, start the platform services locally to have the same infrastructure available as in deployed environments:

```sh
# Enter development shell
nix develop

# Start platform services (Traefik, databases, etc.)
docker compose -f compose.platform.yaml up
```

Platform services provide:

- **Traefik** - Reverse proxy and load balancer (routing, TLS termination)
- **PostgreSQL** - Database server
- **Zitadel** - Identity and access management
- **Mailpit** - Local SMTP sink for email testing

**Platform Dashboard:** http://ingress.localhost (Traefik dashboard)

### Running Application Services

Application services are defined separately and can be run alongside platform services:

```sh
# Start application services (requires platform services running)
docker compose -f compose.yaml up

# Or start everything together
docker compose -f compose.platform.yaml -f compose.yaml up
```

**Compose Files:**

- `compose.platform.yaml` - Platform/infrastructure services (managed by platform team)
- `compose.yaml` - Application services (managed by app teams)

## Deployed Environment

The development environment runs on Hetzner Cloud (Helsinki). After initial provisioning, it automatically reconciles with the `main` branch every minute, pulling changes and redeploying services as needed.

**Platform Services:**

- Traefik Dashboard: https://ingress.platform.sandefjord.kodehode.larsgunnar.no

**Application Services:**

- Example API: https://api-dotnet.platform.sandefjord.kodehode.larsgunnar.no
- Example Next.js UI: https://ui-nextjs.platform.sandefjord.kodehode.larsgunnar.no
- Example Vanilla UI: https://ui-vanilla.platform.sandefjord.kodehode.larsgunnar.no

### Initial Infrastructure Setup

For platform/infrastructure teams provisioning environments (one-time setup):

**Required:** Ensure you have the [prerequisites](#prerequisites) (Hetzner Cloud account, deSEC account, and registered domain) configured before proceeding.

```sh
# Set up GitHub repository
cd project && tofu init && tofu apply

# Provision development environment (one-time)
cd infrastructure/environments/development

# Initialize OpenTofu
tofu init

# Review provisioning plan
tofu plan

# Provision infrastructure (creates Hetzner Cloud server and initial deployment)
tofu apply
```

After provisioning, the server automatically reconciles with the repository:

- Pulls compose files from `infrastructure/environments/development/compose.*.yaml`
- Merges platform and application compose files
- Redeploys services when changes are detected

**Reconciliation:** Changes pushed to the `main` branch are automatically reconciled and redeployed every minute.

## CI/CD

The `publish-oci-manifests.yml` workflow automatically:

- Discovers services with Dockerfiles in `src/`
- Builds multi-platform images (`linux/amd64`, `linux/arm64`)
- Publishes to GitHub Container Registry: `ghcr.io/<owner>/<service-name>`
- Tags: `latest`, branch name, and commit SHA

Built images are automatically used when environments reconcile via the compose files in `infrastructure/environments/`.

## Example Services

- **`example-web-api-service`** - ASP.NET Core API
- **`example-web-ui-nextjs`** - Next.js application
- **`example-web-ui-vanilla`** - Vanilla JavaScript with ES modules

## Infrastructure Modules

**`hetzner-compose-application`** - OpenTofu module for provisioning Hetzner Cloud servers that automatically reconcile Docker Compose applications from Git repositories. Supports multiple compose files to separate platform and application services. After initial provisioning, the server continuously reconciles with the repository.

## Documentation

- [`src/README.md`](src/README.md) - Source code organization
- [`infrastructure/README.md`](infrastructure/README.md) - Infrastructure as Code
- [`project/README.md`](project/README.md) - Project governance
- [`.github/WORKFLOWS.md`](.github/WORKFLOWS.md) - GitHub Actions workflows
