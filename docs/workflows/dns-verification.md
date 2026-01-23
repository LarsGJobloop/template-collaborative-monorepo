# DNS and DNSSEC Verification

This guide provides detailed commands for verifying DNS propagation and DNSSEC configuration after setting up your domain with deSEC.

## Prerequisites

- `dig` command-line tool installed (usually included with `bind-utils` or `dnsutils`)
- Domain configured with deSEC nameservers at your registrar
- DS records added at your domain registrar
- Replace `your-domain.com` with your actual domain in all commands

## Quick Reference

**Basic checks:**
- Nameservers: `dig NS your-domain.com +short`
- A record: `dig A platform.your-domain.com +short`
- DNSSEC validation: `dig A platform.your-domain.com +dnssec @8.8.8.8` (look for `ad` flag)

## DNS Propagation Verification

### Check Nameserver Propagation

Verify that your domain is using deSEC nameservers:

```bash
# Basic check (uses system resolver)
dig NS your-domain.com +short

# Verify via Google's DNS
dig NS your-domain.com @8.8.8.8 +short

# Verify via Cloudflare's DNS
dig NS your-domain.com @1.1.1.1 +short
```

**Expected result:** All queries should return deSEC nameservers (e.g., `ns1.desec.io`, `ns2.desec.org`).

### Check DNS Records via deSEC Nameservers

Verify records are visible directly from deSEC's nameservers (bypasses propagation delays):

```bash
# Query deSEC nameserver directly
dig A platform.your-domain.com @ns1.desec.io +short
dig A *.platform.your-domain.com @ns1.desec.io +short

# Query another deSEC nameserver for redundancy
dig A platform.your-domain.com @ns2.desec.org +short
```

**Expected result:** Should return your Hetzner Cloud server IP address immediately (even if public DNS hasn't propagated yet).

### Check Public DNS Resolution

Once propagated, verify resolution via public DNS servers:

```bash
# System default resolver
dig A platform.your-domain.com +short

# Google's DNS
dig A platform.your-domain.com @8.8.8.8 +short

# Cloudflare's DNS
dig A platform.your-domain.com @1.1.1.1 +short
```

**Expected result:** All should return your server's IP address consistently. If results differ, propagation may still be in progress.

## DNSSEC Verification

### Check DNSSEC Signatures

Verify that DNSSEC signatures are present:

```bash
# Check DNSKEY records (should show KSK and ZSK keys)
dig DNSKEY your-domain.com +dnssec

# Check for RRSIG records (signatures) on A records
dig A platform.your-domain.com +dnssec
```

**What to look for:** **RRSIG records** in the response indicate DNSSEC signing is active.

### Verify DNSSEC Chain of Trust

Check the DS records that link your registrar to deSEC:

```bash
# Check DS records (should match what you added at registrar)
dig DS your-domain.com +dnssec

# Verify via public DNS
dig DS your-domain.com @8.8.8.8 +dnssec
```

**Expected result:** DS records should match what you configured at your domain registrar. If missing, DNSSEC chain is broken.

### Verify DNSSEC Validation

Check if DNSSEC validation is working correctly:

```bash
# Check DNSSEC validation (look for "ad" flag in header)
dig A platform.your-domain.com +dnssec @8.8.8.8

# Detailed validation check (disables checking, shows raw response)
dig A platform.your-domain.com +dnssec +cdflag @8.8.8.8
```

**What to look for:**
- **`ad` flag** in the response header (indicates DNSSEC validation passed)
- **RRSIG records** present in the answer section
- No validation errors in the response

### Complete DNSSEC Chain Verification

Verify the complete chain from registrar to your records:

```bash
# 1. Check DS records at registrar level
dig DS your-domain.com +dnssec @8.8.8.8

# 2. Check DNSKEY at your domain (via deSEC)
dig DNSKEY your-domain.com +dnssec @ns1.desec.io

# 3. Verify signed A records with validation
dig A platform.your-domain.com +dnssec @8.8.8.8
```

**Expected result:** All three should succeed with `ad` flag present in step 3.

## What to Look For

### Successful DNS Propagation

- ✅ Nameservers show deSEC nameservers consistently across all queries
- ✅ A records resolve to your server IP from all nameservers
- ✅ Public DNS servers return consistent results

### Successful DNSSEC Configuration

- ✅ DNSKEY records present for your domain
- ✅ RRSIG records present in responses
- ✅ DS records match what you configured at registrar
- ✅ `ad` flag present in validated queries (indicates authenticated data)
- ✅ No DNSSEC validation errors

## Troubleshooting

### DNS Not Propagating

If nameservers aren't showing deSEC nameservers:

1. Double-check nameserver configuration at your registrar
2. Verify you saved changes at the registrar
3. Wait longer (some registrars take 24-48 hours)
4. Check registrar's DNS status page for any issues

### DNSSEC Validation Failing

If DNSSEC validation isn't working:

1. Verify DS records are correctly added at your registrar (check Terraform outputs)
2. Compare DS records: `dig DS your-domain.com +dnssec` should match registrar
3. Ensure DNSSEC is enabled at your registrar (not just DS records added)
4. Wait for DNSSEC propagation (can take 24-48 hours)
5. Verify DNSKEY records exist: `dig DNSKEY your-domain.com +dnssec @ns1.desec.io`
6. Check for validation errors: `dig A platform.your-domain.com +dnssec @8.8.8.8` (should show `ad` flag)

### Records Not Resolving

If DNS records aren't resolving:

1. Query deSEC nameservers directly: `dig A platform.your-domain.com @ns1.desec.io +short` (bypasses propagation)
2. If deSEC nameservers return correct IP, issue is propagation - wait longer
3. If deSEC nameservers don't return IP, check Terraform outputs and deSEC dashboard
4. Verify server IP matches: `tofu output application_address`
5. Check TTL hasn't expired (wait for refresh if needed)

## Propagation Times

> [!NOTE]
>
> Typical DNS propagation time: 15 minutes to 48 hours, though usually completes within 1-2 hours. DNSSEC propagation may take additional time (up to 24-48 hours in some cases).
