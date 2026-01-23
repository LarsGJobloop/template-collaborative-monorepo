# Template Bootstrap Walkthrough

This guide walks you through setting up a new project from this template repository from scratch.

## Prerequisites

- Git installed
- OpenTofu (or Terraform) installed
- A GitHub account
- A domain name registered with a domain registrar
- Access to your domain registrar's DNS management interface

## Step 1: Clone the Template

Clone the template repository locally:

```bash
git clone https://github.com/LarsGJobloop/template-collaborative-monorepo.git your-project-name
cd your-project-name
```

## Step 2: Update Project Variables

### 2.1 Repository Configuration

Edit `project/remote-repo.tf`:

<details>
<summary>Example configuration</summary>

```hcl
resource "github_repository" "remote_repo" {
  name        = "your-project-name"  # Change this
  description = "Your project description"  # Update this
  visibility  = "public"  # or "private"
}
```

</details>

### 2.2 Repository Collaborators

Edit `project/collaborators.tf`:

<details>
<summary>Example configuration</summary>

```hcl
locals {
  administrators = [
    "your-github-username",  # Add your GitHub username
    # Add more administrators as needed
  ]
  contributors = [
    "contributor-username",  # Add contributor usernames
    # Add more contributors as needed
  ]
}
```

</details>

> [!NOTE]
>
> The repository owner always has admin access automatically, so you don't need to add yourself if you're the owner.

### 2.3 Update Root README

Edit `README.md` and update:
- Repository URLs in badges and links
- Project name and description

### 2.4 Update Domain Configuration

Edit `infrastructure/environments/development/domain.tf`:

<details>
<summary>Example configuration</summary>

```hcl
resource "desec_domain" "domain" {
  name = "your-domain.com"  # Change to your domain
}
```

</details>

## Step 3: GitHub Setup

### 3.1 Create GitHub Personal Access Token

1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens → "Generate new token"
2. Name it (e.g., "Project Bootstrap")
3. Select scopes:
   - `repo` (Full control of private repositories)
   - `admin:org` (if creating org repos)
4. Generate and copy the token

### 3.2 Configure GitHub Token

Create `project/.auto.tfvars`:

<details>
<summary>Example configuration</summary>

```hcl
github_token = "your-github-token-here"
```

</details>

> [!NOTE]
>
> The `.auto.tfvars` file is in `.gitignore` and won't be committed. This token is only needed for initial repository creation and can be removed or scoped down after setup.

## Step 4: Apply Project Configuration

From the repository root, initialize and apply the project configuration:

```bash
cd project
tofu init
tofu plan  # Review the changes
tofu apply  # Create the GitHub repository
```

After applying, note the `remote_url` output. Update your local git remote and push your changes:

```bash
cd ..
git remote set-url origin <remote_url_from_output>
git add -A
git commit -m "Initial commit from template"
git push -u origin main
```

> [!IMPORTANT]
>
> Commit and push your configuration changes before proceeding to infrastructure setup.

## Step 5: deSEC API Token Setup

### 5.1 Create deSEC API Token

1. Go to [deSEC.io](https://desec.io/) and create an account (if needed), then verify your email
2. Go to Settings → API Tokens → "Create Token"
3. Name it (e.g., "Infrastructure Management")
4. Select scope: **Can create domains** (minimum required)
5. Copy the token

### 5.2 Configure deSEC Token

Create `infrastructure/environments/development/.auto.tfvars`:

<details>
<summary>Example configuration</summary>

```hcl
desec_token = "your-desec-token-here"
acme_email  = "your-email@example.com"  # Email for Let's Encrypt certificates
```

</details>

## Step 6: Hetzner Cloud Setup

### 6.1 Create Hetzner Cloud Account and Project

1. Go to [Hetzner Cloud](https://www.hetzner.com/cloud) and sign up
2. Verify your account
3. Create a new project (or use the default) and name it appropriately

### 6.2 Create API Token

1. In your project, go to "Security" → "API Tokens" → "Generate API Token"
2. Name it (e.g., "Infrastructure Management")
3. Select permissions: **Read & Write**
4. Copy the token

### 6.3 Configure Hetzner Token

Add to `infrastructure/environments/development/.auto.tfvars`:

<details>
<summary>Example configuration</summary>

```hcl
hcloud_token = "your-hetzner-token-here"
```

The complete `.auto.tfvars` file should contain:

```hcl
desec_token  = "your-desec-token-here"
hcloud_token = "your-hetzner-token-here"
acme_email   = "your-email@example.com"
```

</details>

## Step 7: Apply Environment Infrastructure

From the repository root, initialize and apply the environment infrastructure:

```bash
cd infrastructure/environments/development
tofu init
tofu plan  # Review the changes (will create server, domain, DNS records)
tofu apply  # Provision the infrastructure
```

This will:
- Create a Hetzner Cloud server
- Configure the domain in deSEC (creates domain if it doesn't exist)
- Set up initial DNS records

## Step 8: Configure Domain Registrar DNS

After applying the infrastructure, configure your domain registrar with the following records from Terraform outputs.

### 8.1 Get DNS Configuration from Terraform Outputs

After `tofu apply` completes, view the outputs:

```bash
tofu output
```

Note the following values you'll need to configure at your registrar:
- **Nameservers (NS records)** - typically `ns1.desec.io`, `ns2.desec.org`, etc.
- **DS records (DNSSEC)** - Delegation Signer records for DNSSEC chain of trust

### 8.2 Configure at Domain Registrar

Log into your domain registrar's control panel and navigate to DNS/DNS Management for your domain. Configure:

**1. Nameservers (NS records)**
- Set custom nameservers to deSEC's nameservers (from Terraform outputs)
- Replace any existing nameservers
- Search your registrar's docs for: "change nameservers" or "custom nameservers"

**2. DS records (DNSSEC)**
- Add the DS records from Terraform outputs
- This enables DNSSEC signing and establishes the chain of trust
- Search your registrar's docs for: "DNSSEC DS records" or "delegation signer records"

Save the changes after configuring both.

## Step 9: Wait for DNS Propagation

DNS changes can take time to propagate. After configuring your domain registrar with deSEC nameservers and DS records, wait for DNS propagation to complete.

> [!NOTE]
>
> Typical propagation time: 15 minutes to 48 hours, though usually completes within 1-2 hours. DNSSEC propagation may take additional time.

For detailed verification commands and DNSSEC chain validation, see [DNS and DNSSEC Verification](dns-verification.md).

## Step 10: Verify Deployment

After DNS propagation completes:

1. Traefik dashboard: `https://ingress.platform.your-domain.com` (should show Traefik UI)
2. Application services: Verify your services are accessible via their configured domains
3. HTTPS: Check that Let's Encrypt certificates are issued (browser should show valid SSL)

## Post-Setup Cleanup

### Remove GitHub Token (Optional)

After the repository is created and collaborators are added, you can:

1. Remove or restrict the GitHub token scope
2. Or delete `project/.auto.tfvars` if you won't need to manage collaborators via Terraform

### Security Notes

> [!CAUTION]
>
> Security best practices:
>
> - Never commit `*.tfvars` files (they're in `.gitignore`)
> - Rotate tokens periodically
> - Use least-privilege token scopes
> - Consider using secret management tools for production
> - Delete tokens if you don't know when you will use them next

## Troubleshooting

### DNS Not Propagating

- Double-check nameserver configuration at registrar
- Verify DS records are correctly added
- Wait longer (some registrars take 24-48 hours)
- Check registrar's DNS status page

### Infrastructure Apply Fails

- Verify all tokens are correct in `.auto.tfvars`
- Check token permissions/scopes
- Ensure domain name matches what you registered
- Check Hetzner Cloud account limits/quota

### Services Not Accessible

- Verify DNS has propagated (see [DNS Verification Guide](dns-verification.md))
- Check server is running: `ssh` into server and check Docker containers
- Review Traefik logs: `docker logs <ingress-container>`
- Verify firewall rules allow traffic on ports 80/443
- Check that compose files exist in the repository at the paths specified in `application.tf`
