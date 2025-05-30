using System.Security.Claims;

namespace Toolkit.Authentication.JwtBearer;

/// <summary>
/// Provides functionality for creating, refreshing, and validating authentication tokens.
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Creates a new authentication token with the specified claims.
    /// </summary>
    /// <param name="claims">An optional collection of <see cref="Claim"/> objects to include in the token.
    /// These typically represent user identity information and authorization data.</param>
    /// <returns>A string containing the generated token.</returns>
    string Create(IEnumerable<Claim>? claims = null);
    
    /// <summary>
    /// Generates a new refresh token.
    /// </summary>
    /// <param name="size">The length of the refresh token in bytes. Default is 32.</param>
    /// <returns>A cryptographically random string suitable for use as a refresh token.</returns>
    string Refresh(int size = 32);

    /// <summary>
    /// Validates whether the specified token is authentic and currently valid.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing true if the token is valid, otherwise false.</returns>
    Task<bool> Validate(string token);
}