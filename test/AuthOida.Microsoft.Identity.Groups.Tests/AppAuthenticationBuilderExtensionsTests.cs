using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public class AppAuthenticationBuilderExtensionsTests
{
    [Fact]
    public void AppAuthenticationBuilderNullThrows()
    {
        Assert.Throws<ArgumentNullException>("authenticationBuilder", () => AuthenticationBuilderExtensions.AddMappedGroups((MicrosoftIdentityWebAppAuthenticationBuilder)null!));
    }

    [Fact]
    public void OidcSchemeNullThrows()
    {
        var appBuilder = GetAppAuthenticationBuilder();

        var exception = Assert.Throws<ArgumentException>(() => appBuilder.AddMappedGroups(_ => { }, null!));
        Assert.Equal("'openIdConnectScheme' cannot be null or empty. (Parameter 'openIdConnectScheme')", exception.Message);
    }

    [Fact]
    public void ShouldConfigureTokenValidatedHookForOidc()
    {
        var appBuilder = GetAppAuthenticationBuilder();

        const string scheme = "TestScheme";
        appBuilder.Services.Configure<OpenIdConnectOptions>(scheme, o =>
        {
            o.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = null
            };
        });
        appBuilder.AddMappedGroups(openIdConnectScheme: scheme);

        var options = appBuilder.GetOptionsInstance<OpenIdConnectOptions>(scheme);
        Assert.NotNull(options.Events?.OnTokenValidated);
    }

    [Fact]
    public void ShouldConfigureOptions()
    {
        var apiBuilder = GetAppAuthenticationBuilder();

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
    public void ShouldRegisterServices()
    {
        var apiBuilder = GetAppAuthenticationBuilder();

        apiBuilder.AddMappedGroups();

        var services = apiBuilder.Services;
        services.AssertRegistered(typeof(GroupsMapper));
        services.AssertRegistered(typeof(IGroupsMapFactory), typeof(GraphGroupsMapFactory), ServiceLifetime.Singleton);
        services.AssertRegistered(typeof(GroupsMapObtainer), serviceLifetime: ServiceLifetime.Singleton);
    }

    private static MicrosoftIdentityWebAppAuthenticationBuilder GetAppAuthenticationBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddAuthentication();

        return builder.AddMicrosoftIdentityWebApp(_ => { }, _ => { });
    }
}
