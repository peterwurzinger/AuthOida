using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public partial class AuthenticationBuilderExtensionsTests
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

        Assert.Throws<ArgumentException>(() => appBuilder.AddMappedGroups(o => { }, null!));
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

        var options = GetOptions<OpenIdConnectOptions>(appBuilder, scheme);
        Assert.NotNull(options.Events?.OnTokenValidated);
    }

    private static MicrosoftIdentityWebAppAuthenticationBuilder GetAppAuthenticationBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddAuthentication();

        var apiBuilder = builder.AddMicrosoftIdentityWebApp(o => { }, j => { });
        return apiBuilder;
    }
}
