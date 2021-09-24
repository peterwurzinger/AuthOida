using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using AuthOida.Microsoft.Identity.Groups;
using System;
using System.Linq;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests
{
    public partial class AuthenticationBuilderExtensionsTests
    {
        private static GroupsMappingOptions GetGroupMappingOptions(MicrosoftIdentityBaseAuthenticationBuilder authBuilder, string authenticationType)
        {
            return GetOptions<GroupsMappingOptions>(authBuilder, authenticationType);
        }

        private static TOptions GetOptions<TOptions>(MicrosoftIdentityBaseAuthenticationBuilder authBuilder, string authenticationType)
            where TOptions : class, new()
        {
            var services = authBuilder.Services.BuildServiceProvider();
            var optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<TOptions>>();
            var options = optionsSnapshot.Get(authenticationType);
            return options;
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

            var options = GetGroupMappingOptions(apiBuilder, "TestScheme");

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
            AssertRegistered(services, typeof(GroupsMapper));
            AssertRegistered(services, typeof(IGroupsMapFactory), typeof(GraphGroupsMapFactory), ServiceLifetime.Singleton);
            AssertRegistered(services, typeof(GroupsMapObtainer), serviceLifetime: ServiceLifetime.Singleton);
        }

        private static void AssertRegistered(IServiceCollection services, Type serviceType, Type? implementationType = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            implementationType ??= serviceType;
            var serviceDescriptor = services.SingleOrDefault(s => s.ServiceType == serviceType && s.ImplementationType == implementationType && s.Lifetime == serviceLifetime);

            Assert.NotNull(serviceDescriptor);
        }
    }
}
