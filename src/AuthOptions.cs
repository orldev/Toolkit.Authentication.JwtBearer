/// <summary>
/// Represents authentication configuration options for token-based authentication.
/// This class contains settings used to configure JWT (JSON Web Token) authentication.
/// </summary>
public class AuthOptions
{
    /// <summary>
    /// Gets or sets the issuer (publisher) of the token.
    /// This identifies the principal that issued the JWT.
    /// </summary>
    public required string Issuer { get; set; } 
    
    /// <summary>
    /// Gets or sets the intended audience (consumer) of the token.
    /// This identifies the recipients that the JWT is intended for.
    /// </summary>
    public required string Audience { get; set; } 
    
    /// <summary>
    /// Gets or sets the secret key used for signing the token.
    /// This should be a secure, randomly generated string.
    /// </summary>
    public required string SecretKey { get; set; } 
    
    /// <summary>
    /// Gets or sets a value indicating whether the audience should be validated during token validation.
    /// When true, the token's audience must match this <see cref="Audience"/> value.
    /// </summary>
    public bool ValidateAudience { get; set; } 
    
    /// <summary>
    /// Gets or sets a value indicating whether the issuer should be validated during token validation.
    /// When true, the token's issuer must match this <see cref="Issuer"/> value.
    /// </summary>
    public bool ValidateIssuer { get; set; } 
    
    /// <summary>
    /// Gets or sets a value indicating whether the token's lifetime should be validated.
    /// When true, checks that the token is not expired and is valid for use at the current time.
    /// </summary>
    public bool ValidateLifetime { get; set; } 
    
    /// <summary>
    /// Gets or sets a value indicating whether the issuer signing key should be validated.
    /// When true, the signing key must be valid and trusted.
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; }
    
    /// <summary>
    /// Gets or sets the lifetime of the token in minutes.
    /// This determines how long the token will remain valid after being issued.
    /// </summary>
    public int TokenLifetime { get; set; } 
}