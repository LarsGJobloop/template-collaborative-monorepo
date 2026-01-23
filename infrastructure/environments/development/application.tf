module "application" {
  source = "../../modules/hetzner-compose-application"

  # Metadata
  application_name = "development"

  # Application Configuration
  git_repository_url    = "https://github.com/LarsGJobloop/template-collaborative-monorepo.git"
  git_repository_branch = "main"
  compose_file_paths = [
    "infrastructure/environments/development/compose.yaml"
  ]
  reconciliation_interval = "1m"

  # Server Configuration
  server_location = "hel1"  # Helsinki, Finland
  server_type     = "cax41" # 32GB RAM, 16 vCPUs, 320GB SSD

  # Admin Configuration
  admin_ssh_key = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIMEgt9VIfTrK/HEBNe+3oWnmYWtzrd9/plYt7Kn8RPfm admin"
}
