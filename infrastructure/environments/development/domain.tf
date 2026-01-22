resource "desec_domain" "domain" {
  name = "sandefjord.kodehode.larsgunnar.no"
}

resource "desec_rrset" "rrset-a" {
  domain  = desec_domain.domain.name
  subname = "platform"
  type    = "A"
  records = [
    module.application.application_address.ipv4
  ]
  ttl = 3600
}
