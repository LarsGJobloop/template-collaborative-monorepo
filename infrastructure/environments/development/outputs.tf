output "application_address" {
  description = "Public IP addresses of the server (IPv4 and IPv6)"
  value       = module.application.application_address
}

output "domain_name" {
  description = "Domain name managed by deSEC"
  value       = desec_domain.domain.name
}
