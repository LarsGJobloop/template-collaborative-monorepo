output "remote_url" {
  description = "The URL of the repository"
  value       = github_repository.remote_repo.http_clone_url
  sensitive   = false
}
