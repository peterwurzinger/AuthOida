using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public class ApiAuthenticationBuilderExtensionsTests
{
    [Fact]
    public void ApiAuthenticationBuilderNullThrows()
    {
        Assert.Throws<ArgumentNullException>("authenticationBuilder", () => AuthenticationBuilderExtensions.AddMappedGroups((MicrosoftIdentityWebApiAuthenticationBuilder)null!));
    }

    [Fact]
    public void JwtBearerSchemeNullThrows()
    {
        var apiBuilder = GetApiAuthenticationBuilder();

        var exception = Assert.Throws<ArgumentException>(() => apiBuilder.AddMappedGroups(_ => { }, null!));
        Assert.Equal("'jwtBearerScheme' cannot be null or empty. (Parameter 'jwtBearerScheme')", exception.Message);
    }

    [Fact]
    public void ShouldConfigureTokenValidatedHookForBearer()
    {
        var apiBuilder = GetApiAuthenticationBuilder();

        const string scheme = "TestScheme";
        apiBuilder.Services.Configure<JwtBearerOptions>(scheme, o =>
        {
            o.Events = new JwtBearerEvents
            {
                OnTokenValidated = _ => Task.CompletedTask
            };
        });
        apiBuilder.AddMappedGroups(jwtBearerScheme: scheme);

        var options = apiBuilder.GetOptionsInstance<JwtBearerOptions>(scheme);
        Assert.NotNull(options.Events?.OnTokenValidated);
    }

    [Fact]
    public void ShouldConfigureOptions()
    {
        var apiBuilder = GetApiAuthenticationBuilder();

        apiBuilder.AddMappedGroups(o =>
        {
            o.AuthenticationType = "Test";
            o.GroupClaimType = "TestClaimType";
            o.TokenGroupClaimType = "TestGroupClaimType";
        }, "TestScheme");

        var options = apiBuilder.GetGroupMappingOptions("TestScheme");

        Assert.Equal("Test", options.AuthenticationType);
        Assert.Equal("TestClaimType", options.GroupClaimType);
        Assert.Equal("TestGroupClaimType", options.TokenGroupClaimType);
    }

    [Fact]
    public void ShouldRegisterServices2()
    {
        var apiBuilder = GetApiAuthenticationBuilder();

        apiBuilder.AddMappedGroups();

        var services = apiBuilder.Services;
        services.AssertRegistered(typeof(GroupsMapper));
        services.AssertRegistered(typeof(IGroupsMapFactory), typeof(GraphGroupsMapFactory), ServiceLifetime.Singleton);
        services.AssertRegistered(typeof(GroupsMapObtainer), serviceLifetime: ServiceLifetime.Singleton);
    }

    private static MicrosoftIdentityWebApiAuthenticationBuilder GetApiAuthenticationBuilder()
    {
#if NET6_0_OR_GREATER
        var host = WebApplication.CreateBuilder();
        var services = host.Services;
#else
        var services = new ServiceCollection();
#endif
        var builder = services.AddAuthentication();

        return builder.AddMicrosoftIdentityWebApi(_ => { }, _ => { });
    }
}
