locals {
  administrators = [
    # NOTE! Repository Owner always has admin access
    "zabronax",
  ]
  contributors = [
    "larsgkodehode",
  ]
}

# Repository Administrators
resource "github_repository_collaborator" "administrators" {
  for_each   = toset(local.administrators)
  repository = github_repository.remote_repo.name
  username   = each.value
  permission = "admin"
}

# Repository Contributors
resource "github_repository_collaborator" "contributors" {
  for_each   = toset(local.contributors)
  repository = github_repository.remote_repo.name
  username   = each.value
  permission = "push"
}
