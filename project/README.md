# Project

This directory contains OpenTofu configuration for managing project-level GitHub infrastructure.

## Resources Managed

- **GitHub Repository** (`remote-repo.tf`): Creates and configures the remote GitHub repository
- **Repository Collaborators** (`collaborators.tf`): Manages repository access with administrators and contributors
- **Outputs** (`outputs.tf`): Exposes the repository URL for use in other configurations

## Usage

1. **Initialize OpenTofu**:
   ```sh
   tofu init
   ```

2. **Review planned changes**:
   ```sh
   tofu plan
   ```

3. **Apply configuration**:
   ```sh
   tofu apply
   ```

## Required Variables

- `github_token`: GitHub personal access token with repository administration permissions
  - Set in `.auto.tfvars` (not committed to version control)

## Outputs

- `remote_url`: The HTTP clone URL of the created repository
