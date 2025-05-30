# Toolkit.Authentication.JwtBearer

A secure, production-ready JWT Bearer authentication extension for ASP.NET Core applications.

## Installation

```bash
dotnet add package Snail.Toolkit.Authentication.JwtBearer
```

## Configuration

### Service Registration

```csharp
// Program.cs
builder.Services.AddAuthJwtBearer(builder.Configuration);
```

### Middleware Setup

```csharp
// Program.cs
app.UseAuthentication();  // Must come before UseAuthorization
app.UseAuthorization();   // Required for [Authorize] attributes
```

### appsettings.json Configuration

```json
{
  "Jwt": {
    "Issuer": "your_issuer",
    "Audience": "your_audience",
    "SecretKey": "minimum_32_character_secure_key_here",
    "ValidateAudience": true,
    "ValidateIssuer": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true,
    "TokenLifetime": 60
  }
}
```

#### Configuration Options

| Setting                  | Required | Default | Description |
|--------------------------|----------|---------|-------------|
| Issuer                   | Yes      | -       | Token publisher identifier |
| Audience                 | Yes      | -       | Intended token recipient |
| SecretKey                | Yes      | -       | Minimum 32-character secure key |
| ValidateAudience         | No       | true    | Validate token audience |
| ValidateIssuer           | No       | true    | Validate token issuer |
| ValidateLifetime         | No       | true    | Validate token expiration |
| ValidateIssuerSigningKey | No       | true    | Validate token signature |
| TokenLifetime            | No       | 60      | Token validity in minutes |

## Advanced Usage

### Custom JWT Bearer Options

```csharp
builder.Services.AddAuthJwtBearer(builder.Configuration, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Custom authentication failure handling
            return Task.CompletedTask;
        }
    };
});
```

### Using with HashiCorp Vault

```csharp
// Example using Vault integration
builder.Services.AddAuthJwtBearer(configuration, vaultOptions =>
{
    vaultOptions.RequireHttpsMetadata = false; // For dev environments only
});
```

## Security Considerations

1. Always use HTTPS in production
2. Rotate SecretKey periodically
3. Set appropriate TokenLifetime based on your security requirements
4. Store SecretKey securely (consider using Azure Key Vault or AWS Secrets Manager)

## Troubleshooting

### Common Issues

- **Clock skew issues**: Consider adjusting `ClockSkew` in validation parameters

## Samples

### 1. **Token Creation Example**
```csharp
// Create a token with user claims
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, "user123"),
    new Claim(ClaimTypes.Name, "John Doe"),
    new Claim(ClaimTypes.Email, "john@example.com"),
    new Claim(ClaimTypes.Role, "Admin")
};

var token = tokenProvider.Create(claims);

// Output: "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
Console.WriteLine($"Generated token: {token}");
```

### 2. **Refresh Token Generation Example**
```csharp
// Generate a standard refresh token (32 bytes)
var refreshToken = tokenProvider.Refresh();

// Generate a longer refresh token (64 bytes)
var longRefreshToken = tokenProvider.Refresh(64); 

// Output: "m6sla9PxYW3T4XJkqO7nBw2v..." (Base64 string)
Console.WriteLine($"Refresh token: {refreshToken}");
```

### 3. **Token Validation Example**
```csharp
// Validate a token (async)
var isValid = await tokenProvider.Validate(token);

if (isValid)
{
    Console.WriteLine("Token is valid");
    var principal = new JwtSecurityTokenHandler().ValidateToken(token, 
        new TokenValidationParameters(), out _);
    Console.WriteLine($"User: {principal.Identity.Name}");
}
else
{
    Console.WriteLine("Invalid token");
}
```

### 4. **Integration with Controllers**
```csharp
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenProvider _tokenProvider;

    public AuthController(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Authentication logic...
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim("custom_claim", "value")
        };

        return Ok(new
        {
            Token = _tokenProvider.Create(claims),
            RefreshToken = _tokenProvider.Refresh(),
            ExpiresIn = TimeSpan.FromMinutes(60).TotalSeconds
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        if (!await ValidateRefreshToken(request.RefreshToken))
            return Unauthorized();

        var newToken = _tokenProvider.Create(GetUserClaims());
        return Ok(new { Token = newToken });
    }
}
```

## License

Toolkit.Authentication.JwtBearer is a free and open source project, released under the permissible [MIT license](LICENSE).
