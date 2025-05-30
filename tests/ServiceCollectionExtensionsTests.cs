using Microsoft.Extensions.Options;

namespace Toolkit.Authentication.JwtBearer.Tests;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services = new ServiceCollection()
        .AddAuthJwtBearer(Helper.GetConfiguration());
    
    [Fact]
    public void AddToServices_ReturnContains()
    {
        Assert.Contains(_services, d => d.ServiceType == typeof(ITokenProvider));
    }
    
    [Fact]
    public void GetFromServices_Client_ReturnNotNull()
    {
        using var serviceProvider = _services.BuildServiceProvider();
        
        var client = serviceProvider.GetService<ITokenProvider>();
        
        Assert.NotNull(client);
    }
    
    [Fact]
    public void Match_OptionsWithAppSettings_ReturnEqual()
    {
        using var serviceProvider = _services.BuildServiceProvider();
        
        var options = serviceProvider.GetService<IOptions<AuthOptions>>();
        Assert.NotNull(options);

        var value = options.Value;
        
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.Issuer)}"], value.Issuer);
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.Audience)}"], value.Audience);
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.SecretKey)}"], value.SecretKey);
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.ValidateAudience)}"], value.ValidateAudience.ToString().ToLower());
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.ValidateIssuer)}"], value.ValidateIssuer.ToString().ToLower());
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.ValidateLifetime)}"], value.ValidateLifetime.ToString().ToLower());
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.ValidateIssuerSigningKey)}"], value.ValidateIssuerSigningKey.ToString().ToLower());
        Assert.Equal(Helper.InMemorySettings[$"{Helper.Client}:{nameof(value.TokenLifetime)}"], value.TokenLifetime.ToString());
    }
}