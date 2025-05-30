using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Toolkit.Authentication.JwtBearer;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure JWT Bearer authentication.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures JWT Bearer authentication services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application's configuration containing the JWT settings section.</param>
    /// <param name="args">An optional action to configure additional <see cref="JwtBearerOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when required JWT configuration values (Issuer, Audience, or SecretKey) are missing or empty.
    /// </exception>
    /// <remarks>
    /// This method performs the following operations:
    /// <list type="bullet">
    /// <item><description>Binds JWT configuration from the "Jwt" configuration section</description></item>
    /// <item><description>Validates required JWT configuration values</description></item>
    /// <item><description>Configures JWT Bearer authentication with default validation parameters</description></item>
    /// <item><description>Adds authorization services</description></item>
    /// <item><description>Registers the <see cref="ITokenProvider"/> implementation</description></item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddAuthJwtBearer(        
        this IServiceCollection services, 
        IConfiguration configuration,
        Action<JwtBearerOptions>? args = null)
    {
        var configurationSection = configuration.GetSection("Jwt");
        var authOptions = configurationSection.Get<AuthOptions>();
        
        services.Configure<AuthOptions>(configurationSection);
        
        // Validate required configuration values
        ArgumentException.ThrowIfNullOrEmpty(authOptions?.Issuer);
        ArgumentException.ThrowIfNullOrEmpty(authOptions.Audience);
        ArgumentException.ThrowIfNullOrEmpty(authOptions.SecretKey);
        
        // Create security key from the configured secret
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SecretKey));
        
        // Configure JWT Bearer authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, args + (options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // indicates whether the publisher will be validated when validating the token
                    ValidateIssuer = authOptions.ValidateIssuer,
                    // a string representing the publisher
                    ValidIssuer = authOptions.Issuer,

                    // will the token consumer be validated
                    ValidateAudience = authOptions.ValidateAudience,
                    // installing a token consumer
                    ValidAudience = authOptions.Audience,
                    // whether the existence time will be validated
                    ValidateLifetime = authOptions.ValidateLifetime,

                    // security key installation
                    IssuerSigningKey = key,
                    // security key validation
                    ValidateIssuerSigningKey = authOptions.ValidateIssuerSigningKey
                };
            }));
        
        // Add supporting services
        services.AddAuthorization();
        services.TryAddSingleton<ITokenProvider, TokenProvider>();
        
        return services;
    }
}