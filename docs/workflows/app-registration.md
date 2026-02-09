# Application Registration

This guide explains how to register applications with Zitadel for authentication and authorization using OAuth 2.0 flows.

> [!NOTE]
>
> - Services are only available while the platform stack is running. Check Docker Desktop/OrbStack to verify services are running.
> - If volumes are deleted, Zitadel needs to be configured again (manual setup required).
> - Default Zitadel admin credentials: `admin@platform` / `Password1!`

## Local Development Setup

### Starting Platform Services

1. **Start the platform services:**

   ```sh
   docker compose -f compose.platform.yaml up
   ```

2. **Verify services are running:**
   - Traefik Dashboard: http://ingress.localhost
   - Zitadel Console: http://zitadel.localhost
   - Mailpit (email testing): http://mailpit.localhost

3. **Access Zitadel:**
   - Navigate to http://zitadel.localhost
   - Log in with: `admin@platform` / `Password1!`
   - Organization: `platform`

### Resetting the Environment

To completely reset Zitadel and start fresh:

```sh
# Stop services
docker compose -f compose.platform.yaml down

# Remove volumes (this deletes all Zitadel data)
docker compose -f compose.platform.yaml down -v

# Start again (Zitadel will reinitialize)
docker compose -f compose.platform.yaml up
```

> [!WARNING]
> Deleting volumes will remove all registered applications, users, and configurations. You'll need to re-register everything.

## Registering a New Application

### Application Types

All application types in this template use **PKCE (Proof Key for Code Exchange)** OAuth flow, which is the recommended and most secure approach for modern applications:

- **CLI Application (PKCE)**: .NET Console App
- **Web SPA (PKCE)**: Vanilla JavaScript / React
- **Fullstack (PKCE)**: Next.js / .NET Razor Pages
- **API (PKCE)**: ASP.NET Core API

### Why PKCE?

PKCE is preferred because:

- **Enhanced Security**: Prevents authorization code interception attacks
- **No Client Secret Required**: Safer for public clients (SPAs, mobile apps)
- **Industry Standard**: Recommended by OAuth 2.1 specification
- **Works Everywhere**: Suitable for all application types

## Step-by-Step: Registering an Application in Zitadel

### 1. Access Zitadel Console

1. Navigate to http://zitadel.localhost
2. Log in with `admin@platform` / `Password1!`
3. Select the `platform` organization

### 2. Create a New Application

1. Go to **Applications** in the sidebar
2. Click **+ New** or **Create Application**
3. Choose **User Agent** (for web apps) or **Native** (for CLI/mobile apps)
4. Enter a name for your application (e.g., "My Web App")
5. Click **Continue**

### 3. Configure OAuth Settings

1. **Application Type**: Select based on your app:
   - **User Agent**: For web SPAs (React, Vanilla JS)
   - **Native**: For CLI applications or mobile apps
   - **Web**: For server-side applications (Next.js SSR, Razor Pages)

2. **Redirect URIs**: Add your callback URLs
   - **Local Development**: `http://localhost:PORT/callback` or `http://127.0.0.1:PORT/callback`
   - **Public Deployment**: `https://yourdomain.com/callback`
   - **Example**: `http://localhost:3000/auth/callback`

3. **Post Logout Redirect URIs** (optional):
   - Where users should be redirected after logout
   - **Example**: `http://localhost:3000/` or `https://yourdomain.com/`

4. **Response Types**: Select `code` (Authorization Code flow)

5. **Grant Types**: Select `authorization_code`

6. **Auth Method**: Select `none` (PKCE doesn't require client secret)

7. **Access Token Type**: Select `Bearer`

8. **PKCE**: Enable **Code Challenge Method** → Select `S256` (SHA256)

### 4. Save and Get Credentials

1. Click **Create** or **Save**
2. **Copy the Client ID** - you'll need this in your application configuration
3. Note: With PKCE, you don't need a client secret

### 5. Configure Your Application

Add the Client ID to your application configuration:

**Environment Variables:**

```bash
ZITADEL_CLIENT_ID=your-client-id-here
ZITADEL_AUTHORITY=http://zitadel.localhost
ZITADEL_REDIRECT_URI=http://localhost:3000/auth/callback
```

**Configuration File:**

```json
{
  "Zitadel": {
    "ClientId": "your-client-id-here",
    "Authority": "http://zitadel.localhost",
    "RedirectUri": "http://localhost:3000/auth/callback"
  }
}
```

## OAuth 2.0 Flow: PKCE (Proof Key for Code Exchange)

### Overview

PKCE is an OAuth 2.0 security extension that makes authorization flows more secure, especially for public clients. Here's how it works:

### Step-by-Step Flow

#### 1. **User Initiates Login**

When an anonymous user tries to access a protected resource:

- Generate a random `code_verifier` (43-128 characters)
- Create `code_challenge` = SHA256(code_verifier) encoded as base64url
- Store `code_verifier` securely (session storage, memory, or encrypted cookie)
- Redirect user to Zitadel authorization endpoint with `code_challenge` and other OAuth parameters

**Key Parameters:**

- `client_id`: Your application's Client ID
- `redirect_uri`: Your callback URL (must match Zitadel configuration)
- `response_type`: `code`
- `scope`: `openid profile email` (or other required scopes)
- `code_challenge`: The SHA256 hash of your code verifier
- `code_challenge_method`: `S256`
- `state`: Random value for CSRF protection

#### 2. **User Authenticates**

- User is redirected to Zitadel login page
- User enters credentials (`admin@platform` / `Password1!`)
- User grants permissions to your application
- Zitadel redirects back to your callback URL with an authorization code

#### 3. **Handle Callback**

Your callback endpoint receives the authorization code:

```
GET /auth/callback?code=AUTHORIZATION_CODE&state=STATE_VALUE
```

**What to do:**

1. **Verify the state parameter** (prevents CSRF attacks)
   - Compare received state with the one you sent
   - If they don't match, reject the request

2. **Exchange code for tokens**
   - Make a POST request to Zitadel's token endpoint
   - Include the authorization code, client ID, redirect URI, and the original `code_verifier`
   - Zitadel verifies the code verifier matches the code challenge

3. **Receive tokens**
   - Access token: Used for authenticated API requests
   - ID token: Contains user identity information (JWT)
   - Refresh token: Used to obtain new access tokens (if `offline_access` scope requested)

#### 4. **Verify Token**

Always verify tokens before trusting them:

1. **Verify signature**: Ensure token is signed by Zitadel (using JWKS endpoint)
2. **Verify issuer**: Check `iss` claim matches `http://zitadel.localhost`
3. **Verify audience**: Check `aud` claim matches your Client ID
4. **Check expiration**: Verify `exp` claim hasn't passed

#### 5. **Use Token**

Once verified, use the access token to make authenticated requests by including it in the `Authorization` header as a Bearer token.

### Anonymous vs Authenticated Boundary

**Decision Point:** Where does your application require authentication?

**Common Patterns:**

1. **Public Landing Page → Protected App**
   - Landing page: `/` (public, no auth)
   - App routes: `/app/*` (protected, requires auth)
   - Redirect anonymous users to login when accessing `/app/*`

2. **Public API → Protected API**
   - Public endpoints: `/api/public/*` (no auth)
   - Protected endpoints: `/api/protected/*` (requires auth)
   - Return 401 Unauthorized if no valid token

3. **Full Protection**
   - All routes require authentication
   - Redirect **every** anonymous request to login
   - Only allow access after successful authentication

**Implementation Approach:**

- Check for valid token in request headers
- If no token or invalid token, redirect to login (web) or return 401 (API)
- Verify token signature and claims before allowing access

## Security Best Practices

### Token Storage

> [!CAUTION]
>
> **Be very wary of persisting tokens** unless you add a Vault-like service.
> While hashing is simple, proving that you do it correctly to auditors is not.

**Recommendations:**

- **SPAs (Browser)**: Store tokens in memory or sessionStorage (not localStorage)
- **Server-Side**: Store tokens in encrypted cookies or server-side session
- **CLI Apps**: Store tokens in encrypted keychain/credential store
- **Never**: Store tokens in plain text, version control, or client-side localStorage for sensitive apps

### Scope and Permissions

> [!NOTE]
>
> Be mindful of:
>
> - **Least Privileges**: Don't request access to more scopes than you need
>   - Use `openid profile email` for basic user info
>   - Only request additional scopes if absolutely necessary
> - **Overly Complex Authorization Models**: If going beyond ownership semantics, bring your Lawyer and Legislator goggles
>   - Complex authorization can lead to security vulnerabilities
>   - Consider using Zitadel's built-in role and permission features

### Common Scopes

- `openid` - Required for OpenID Connect
- `profile` - User's profile information (name, etc.)
- `email` - User's email address
- `offline_access` - Request refresh token for long-lived sessions

## Troubleshooting

### Common Issues

1. **"Invalid redirect URI"**
   - Ensure the redirect URI in your app matches exactly what's configured in Zitadel
   - Check for trailing slashes, http vs https, localhost vs 127.0.0.1

2. **"Invalid code verifier"**
   - Ensure you're using the same `code_verifier` that generated the `code_challenge`
   - Verify the code challenge method matches (S256)

3. **"Token verification failed"**
   - Check that you're fetching JWKS from the correct Zitadel instance
   - Verify the issuer matches your Zitadel domain
   - Ensure your system clock is synchronized

4. **"CORS errors"**
   - Zitadel should handle CORS automatically, but verify your redirect URIs are configured
   - Check browser console for specific CORS error details

### Testing Email Verification

Since Zitadel requires email verification for account creation:

1. Check Mailpit at http://mailpit.localhost
2. Look for verification emails sent to test accounts
3. Click verification links from Mailpit interface

## Next Steps

- Review example implementations in `src/example-web-*` directories
- Check Zitadel documentation: https://zitadel.com/docs
- Explore OAuth 2.0 PKCE specification: https://oauth.net/2/pkce/

## Minimal Examples

### JavaScript/TypeScript Example

```javascript
// Configuration
const CLIENT_ID = process.env.ZITADEL_CLIENT_ID;
const AUTHORITY = process.env.ZITADEL_AUTHORITY || 'http://zitadel.localhost';
const REDIRECT_URI = process.env.ZITADEL_REDIRECT_URI || 'http://localhost:3000/auth/callback';

// Generate PKCE code verifier and challenge
function generateCodeVerifier() {
  const array = new Uint8Array(32);
  crypto.getRandomValues(array);
  return base64UrlEncode(array);
}

async function generateCodeChallenge(verifier) {
  const encoder = new TextEncoder();
  const data = encoder.encode(verifier);
  const digest = await crypto.subtle.digest('SHA-256', data);
  return base64UrlEncode(new Uint8Array(digest));
}

function base64UrlEncode(buffer) {
  return btoa(String.fromCharCode(...buffer))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=/g, '');
}

// Step 1: Initiate login
async function initiateLogin() {
  const codeVerifier = generateCodeVerifier();
  const codeChallenge = await generateCodeChallenge(codeVerifier);
  const state = generateCodeVerifier(); // Random state for CSRF protection

  // Store verifier and state securely
  sessionStorage.setItem('code_verifier', codeVerifier);
  sessionStorage.setItem('oauth_state', state);

  const scopes = [ "openid", "profile" "email" ]
  const authUrl = `${AUTHORITY}/oauth/v2/authorize` +
    `?client_id=${CLIENT_ID}` +
    `&redirect_uri=${encodeURIComponent(REDIRECT_URI)}` +
    `&response_type=code` +
    `&scope=${scopes.join(" ")}` +
    `&code_challenge=${codeChallenge}` +
    `&code_challenge_method=S256` +
    `&state=${state}`;

  window.location.href = authUrl;
}

// Step 2: Handle callback
async function handleCallback() {
  const urlParams = new URLSearchParams(window.location.search);
  const code = urlParams.get('code');
  const state = urlParams.get('state');
  const storedState = sessionStorage.getItem('oauth_state');
  const codeVerifier = sessionStorage.getItem('code_verifier');

  // Verify state
  if (state !== storedState) {
    throw new Error('Invalid state parameter');
  }

  // Exchange code for tokens
  const tokenResponse = await fetch(`${AUTHORITY}/oauth/v2/token`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: CLIENT_ID,
      code: code,
      redirect_uri: REDIRECT_URI,
      code_verifier: codeVerifier,
    }),
  });

  const tokens = await tokenResponse.json();

  // Clean up
  sessionStorage.removeItem('code_verifier');
  sessionStorage.removeItem('oauth_state');

  return tokens;
}

// Step 3: Verify and use token
async function verifyToken(accessToken) {
  // Fetch JWKS from Zitadel
  const jwksResponse = await fetch(`${AUTHORITY}/.well-known/jwks.json`);
  const jwks = await jwksResponse.json();

  // Decode token header to get key ID
  const [headerB64] = accessToken.split('.');
  const header = JSON.parse(atob(headerB64));

  // Find matching key
  const key = jwks.keys.find(k => k.kid === header.kid);
  if (!key) {
    throw new Error('Key not found');
  }

  // Verify token (use a JWT library like jose or jsonwebtoken in production)
  // This is a simplified example - use a proper JWT library
  return { verified: true, token: accessToken };
}

// Use token for authenticated requests
async function makeAuthenticatedRequest(url, accessToken) {
  const response = await fetch(url, {
    headers: {
      'Authorization': `Bearer ${accessToken}`,
    },
  });
  return response.json();
}
```

### C# .NET Example

```csharp
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

// Configuration
public class ZitadelConfig
{
    public string ClientId { get; set; } = Environment.GetEnvironmentVariable("ZITADEL_CLIENT_ID");
    public string Authority { get; set; } = Environment.GetEnvironmentVariable("ZITADEL_AUTHORITY") ?? "http://zitadel.localhost";
    public string RedirectUri { get; set; } = Environment.GetEnvironmentVariable("ZITADEL_REDIRECT_URI") ?? "http://localhost:3000/auth/callback";
}

// PKCE helper
public class PkceHelper
{
    public static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Base64UrlEncode(bytes);
    }

    public static string GenerateCodeChallenge(string verifier)
    {
        using (var sha256 = SHA256.Create())
        {
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            return Base64UrlEncode(challengeBytes);
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

// Step 1: Initiate login (ASP.NET Core example)
public class AuthController : Controller
{
    private readonly ZitadelConfig _config;

    [HttpGet("/auth/login")]
    public IActionResult Login()
    {
        var codeVerifier = PkceHelper.GenerateCodeVerifier();
        var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
        var state = PkceHelper.GenerateCodeVerifier();

        // Store in session
        HttpContext.Session.SetString("code_verifier", codeVerifier);
        HttpContext.Session.SetString("oauth_state", state);

        var authUrl = $"{_config.Authority}/oauth/v2/authorize" +
            $"?client_id={HttpUtility.UrlEncode(_config.ClientId)}" +
            $"&redirect_uri={HttpUtility.UrlEncode(_config.RedirectUri)}" +
            $"&response_type=code" +
            $"&scope=openid profile email" +
            $"&code_challenge={codeChallenge}" +
            $"&code_challenge_method=S256" +
            $"&state={state}";

        return Redirect(authUrl);
    }

    // Step 2: Handle callback
    [HttpGet("/auth/callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        var storedState = HttpContext.Session.GetString("oauth_state");
        var codeVerifier = HttpContext.Session.GetString("code_verifier");

        // Verify state
        if (state != storedState)
        {
            return BadRequest("Invalid state parameter");
        }

        // Exchange code for tokens
        using var httpClient = new HttpClient();
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", _config.ClientId },
            { "code", code },
            { "redirect_uri", _config.RedirectUri },
            { "code_verifier", codeVerifier }
        };

        var content = new FormUrlEncodedContent(tokenRequest);
        var response = await httpClient.PostAsync($"{_config.Authority}/oauth/v2/token", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var tokens = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        // Clean up session
        HttpContext.Session.Remove("code_verifier");
        HttpContext.Session.Remove("oauth_state");

        // Store token securely (use encrypted cookie or server-side session)
        HttpContext.Session.SetString("access_token", tokens.AccessToken);

        return Redirect("/");
    }
}

// Token response model
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }
}

// Step 3: Token verification middleware (simplified)
public class TokenVerificationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ZitadelConfig _config;

    public TokenVerificationMiddleware(RequestDelegate next, ZitadelConfig config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip verification for public routes
        if (context.Request.Path.StartsWithSegments("/auth") ||
            context.Request.Path == "/")
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        // Verify token (use a JWT library like System.IdentityModel.Tokens.Jwt)
        // This is a simplified example - use proper JWT validation in production
        var isValid = await VerifyTokenAsync(token);

        if (!isValid)
        {
            context.Response.StatusCode = 401;
            return;
        }

        await _next(context);
    }

    private async Task<bool> VerifyTokenAsync(string token)
    {
        // Fetch JWKS and verify token signature
        // Use System.IdentityModel.Tokens.Jwt for production
        // This is a placeholder
        return true;
    }
}

// Use token for authenticated API requests
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;

    public ApiClient(HttpClient httpClient, string accessToken)
    {
        _httpClient = httpClient;
        _accessToken = accessToken;
    }

    public async Task<T> GetAsync<T>(string url)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content);
    }
}
```

> [!NOTE]
> These examples are minimal implementations for demonstration. In production, use established libraries:
>
> - **JavaScript**: `oidc-client-js`, `@azure/msal-browser`, or `@auth0/auth0-spa-js`
> - **C# .NET**: `Microsoft.AspNetCore.Authentication.OpenIdConnect` or `IdentityModel.OidcClient`
