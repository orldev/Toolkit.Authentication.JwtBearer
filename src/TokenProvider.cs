using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Toolkit.Authentication.JwtBearer;

/// <summary>
/// Provides implementation for JWT (JSON Web Token) creation, refresh token generation, and token validation.
/// </summary>
/// <remarks>
/// This class implements <see cref="ITokenProvider"/> and handles all JWT-related operations
/// using the configuration provided in <see cref="AuthOptions"/>.
/// </remarks>
/// <param name="options">The authentication options containing JWT configuration settings.</param>
public class TokenProvider(IOptions<AuthOptions> options) : ITokenProvider
{
    private readonly AuthOptions _options = options.Value;
    
    /// <summary>
    /// Creates a new JWT (JSON Web Token) with the specified claims.
    /// </summary>
    /// <param name="claims">An optional collection of <see cref="Claim"/> objects representing the user's identity and permissions.</param>
    /// <returns>A string containing the generated JWT.</returns>
    /// <remarks>
    /// The generated token will include:
    /// <list type="bullet">
    /// <item><description>The configured issuer and audience from <see cref="AuthOptions"/></description></item>
    /// <item><description>Current UTC time as the "not before" time</description></item>
    /// <item><description>Expiration time based on <see cref="AuthOptions.TokenLifetime"/></description></item>
    /// <item><description>All provided claims</description></item>
    /// <item><description>HMAC-SHA512 signature using the configured secret key</description></item>
    /// </list>
    /// </remarks>
    public string Create(IEnumerable<Claim>? claims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_options.TokenLifetime)),
            signingCredentials: credentials
        );
        
        var handler = new JwtSecurityTokenHandler();
        var tokenString = handler.WriteToken(token);
        
        return tokenString;
    }
    
    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    /// <param name="size">The size of the refresh token in bytes. Default is 32 (recommended for security).</param>
    /// <returns>A base64-encoded string representing the refresh token.</returns>
    /// <remarks>
    /// This method uses <see cref="RandomNumberGenerator"/> to generate a cryptographically
    /// secure random number which is then converted to a base64 string.
    /// The default size of 32 bytes (256 bits) provides adequate security for most applications.
    /// </remarks>
    public string Refresh(int size = 32)
    {
        var randomNumber = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates a JWT token against the configured authentication options.
    /// </summary>
    /// <param name="token">The JWT token string to validate.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, containing true if the token is valid,
    /// false if validation fails.
    /// </returns>
    /// <remarks>
    /// Validation includes checking:
    /// <list type="bullet">
    /// <item><description>Issuer (if <see cref="AuthOptions.ValidateIssuer"/> is true)</description></item>
    /// <item><description>Audience (if <see cref="AuthOptions.ValidateAudience"/> is true)</description></item>
    /// <item><description>Lifetime/expiration (if <see cref="AuthOptions.ValidateLifetime"/> is true)</description></item>
    /// <item><description>Signature (if <see cref="AuthOptions.ValidateIssuerSigningKey"/> is true)</description></item>
    /// </list>
    /// Note: Clock skew is not adjusted by default (tokens expire exactly at expiration time).
    /// </remarks>
    public async Task<bool> Validate(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = _options.ValidateIssuer,
            ValidIssuer = _options.Issuer,
            ValidateAudience = _options.ValidateAudience,
            ValidAudience = _options.Audience,
            ValidateLifetime = _options.ValidateLifetime,
            ValidateIssuerSigningKey = _options.ValidateIssuerSigningKey,
            IssuerSigningKey = key
            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            // ClockSkew = TimeSpan.Zero
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validatedToken = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        return validatedToken.IsValid;
    }
}