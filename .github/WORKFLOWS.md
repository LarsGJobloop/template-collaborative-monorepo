# GitHub Configuration

This folder contains GitHub-specific configuration files. While you can configure various aspects of the GitHub repository from here, this folder is primarily used for Continuous Integration (CI), Continuous Delivery (CD), and GitHub Actions workflows.

> [!CAUTION]
>
> While it's tempting to automate everything using ready-made actions from the marketplace, be mindful of what secrets are available to workflows. **Always** explicitly set workflow permissions (GitHub's default permissions are very permissive).

## Subdirectories

- **workflows/**: GitHub Actions workflow files that run automatically when repository events occur
  - Example: Run tests when a new commit is pushed to a branch
  
- **actions/**: Reusable custom actions that can be used by workflows
  - Example: `publish-to-docker-hub` or `post-to-discord`
