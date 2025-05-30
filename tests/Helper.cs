using Microsoft.Extensions.Configuration;

namespace Toolkit.Authentication.JwtBearer.Tests;

public static class Helper
{
    public const string Client = "Jwt";
    
    public static readonly Dictionary<string, string> InMemorySettings = new(){
        {$"{Client}:Issuer", "12"},
        {$"{Client}:Audience", "12"},
        {$"{Client}:SecretKey", "ayjN7KaHE2gd2cXrG2j4wyMUP7NX8SYKZxAKm0FYo3ajNKYY3h+CQ4OYnv2WF6It"},
        {$"{Client}:ValidateAudience", "true"},
        {$"{Client}:ValidateIssuer", "true"},
        {$"{Client}:ValidateLifetime", "true"},
        {$"{Client}:ValidateIssuerSigningKey", "true"},
        {$"{Client}:TokenLifetime", "60"}
    };

    public static IConfiguration GetConfiguration()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(InMemorySettings!)
            .Build();

        return configuration;
    }
}