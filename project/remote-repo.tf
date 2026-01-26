resource "github_repository" "remote_repo" {
  name       = "template-collaborative-monorepo"
  visibility = "public"

  homepage_url = "https://platform.sandefjord.kodehode.larsgunnar.no"
  description  = "Template for managing multiple applications and services in a single repository with OpenTofu infrastructure-as-code, Nix development environments, Docker Compose orchestration, and automated CI/CD deployment."
  topics = [
    "monorepo",
    "template",
    "opentofu",
    "nix",
    "docker-compose",
    "infrastructure-as-code"
  ]
}
