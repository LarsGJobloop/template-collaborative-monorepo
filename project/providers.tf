terraform {
  required_providers {
    github = {
      source  = "integrations/github"
      version = "6.10.2"
    }
  }
}

variable "github_token" {
  description = "Token for administering the repository"
  type        = string
  sensitive   = true
}

provider "github" {
  token = var.github_token
}
