# Project

This directory contains Terraform configuration for managing project-level GitHub infrastructure.

## Resources Managed

- **GitHub Repository** (`remote-repo.tf`): Creates and configures the remote GitHub repository
- **Repository Collaborators** (`collaborators.tf`): Manages repository access with administrators and contributors
- **Outputs** (`outputs.tf`): Exposes the repository URL for use in other configurations

## Usage

1. **Initialize Terraform**:
   ```sh
   terraform init
   ```

2. **Review planned changes**:
   ```sh
   terraform plan
   ```

3. **Apply configuration**:
   ```sh
   terraform apply
   ```

> [!NOTE]
>
> When initializing the IaC providers, not all of them might be locked properly. Meaning running the same command on a different platform requires of the lockfile to work. To minimize this you can explicitly locak the providers across multi

## Required Variables

- `github_token`: GitHub personal access token with repository administration permissions
  - Set in `.auto.tfvars` (not committed to version control)

## Outputs

- `remote_url`: The HTTP clone URL of the created repository
