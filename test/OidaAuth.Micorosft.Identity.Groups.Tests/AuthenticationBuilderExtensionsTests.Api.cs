using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using OidaAuth.Microsoft.Identity.Groups;
using System;
using Xunit;

namespace OidaAuth.Micorosft.Identity.Groups.Tests
{
    public partial class AuthenticationBuilderExtensionsTests
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

            Assert.Throws<ArgumentException>(() => apiBuilder.AddMappedGroups(o => { }, null!));
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
                    OnTokenValidated = null
                };
            });
            apiBuilder.AddMappedGroups(jwtBearerScheme: scheme);

            var options = GetOptions<JwtBearerOptions>(apiBuilder, scheme);
            Assert.NotNull(options.Events?.OnTokenValidated);
        }

        private static MicrosoftIdentityWebApiAuthenticationBuilder GetApiAuthenticationBuilder()
        {
            var services = new ServiceCollection();
            var builder = services.AddAuthentication();

            var apiBuilder = builder.AddMicrosoftIdentityWebApi(o => { }, j => { });
            return apiBuilder;
        }
    }
}
