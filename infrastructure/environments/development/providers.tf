terraform {
  required_providers {
    hcloud = {
      source  = "hetznercloud/hcloud"
      version = "1.59.0"
    }

    desec = {
      source  = "Valodim/desec"
      version = "0.6.1"
    }

    local = {
      source  = "hashicorp/local"
      version = "2.5.1"
    }
  }
}

variable "hcloud_token" {
  description = "Hetzner Cloud API token"
  type        = string
  sensitive   = true
}

provider "hcloud" {
  token = var.hcloud_token
}

variable "desec_token" {
  description = "DESec API token"
  type        = string
  sensitive   = true
}

provider "desec" {
  api_token = var.desec_token
}

variable "acme_email" {
  description = "Email address for Let's Encrypt ACME registration"
  type        = string
}
