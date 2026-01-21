# Infrastructure

This directory contains infrastructure code that we manage and provide for others.

## Structure

### Environments

The `environments/` folder contains the actual deployments we manage. The exact structure depends on your project's needs, costs, and requirements. Common environments include:

- **Development**: A sandbox environment with no external obligations. Safe to break things and experiment.
- **Staging**: A non-critical environment for testing and verifying everything works before releasing to production.
- **Preview**: A custom environment for one or more customers to review and discuss changes.
- **Production**: The live environment that serves customers. This is what you're selling and may be contractually obligated to maintain. Handle with care!

### Modules

The `modules/` folder contains shared patterns and reusable components that simplify creating deployments.
