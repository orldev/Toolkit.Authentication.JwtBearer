using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Toolkit.Authentication.JwtBearer.Tests;

public class ServiceTests
{
    private readonly IServiceCollection _services = new ServiceCollection()
        .AddAuthJwtBearer(Helper.GetConfiguration());
    
    [Fact]
    public async Task CreateAndValidate_ReturnTrue()
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Name, "User1"),
            new (JwtRegisteredClaimNames.Sub, "email@email.test"),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (ClaimTypes.Role, "Admin")
        };
        Assert.NotNull(claims);

        await using var serviceProvider = _services.BuildServiceProvider();
        
        var client = serviceProvider.GetService<ITokenProvider>();
        Assert.NotNull(client);
        
        var token = client.Create(claims);
        Assert.NotNull(token);

        var validate = await client.Validate(token);
        Assert.True(validate);
    }
    
    [Fact]
    public async Task CreateAndValidate_ReturnFalse()
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Name, "User1"),
            new Claim(JwtRegisteredClaimNames.Sub, "email@email.test"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (ClaimTypes.Role, "Admin")
        };
        Assert.NotNull(claims);

        await using var serviceProvider = _services.BuildServiceProvider();
        
        var client = serviceProvider.GetService<ITokenProvider>();
        Assert.NotNull(client);
        
        var token = client.Create(claims);
        Assert.NotNull(token);

        var failedToken = token.Remove(0);
        
        var validate = await client.Validate(failedToken);
        Assert.False(validate);
    }
    
    [Fact]
    public void CreateRefresh_ReturnNotEmpty()
    {
        const int size = 32;
        using var serviceProvider = _services.BuildServiceProvider();
        
        var client = serviceProvider.GetService<ITokenProvider>();
        Assert.NotNull(client);
        
        var token = client.Refresh(size);
        
        Assert.NotEmpty(token);
    }
}