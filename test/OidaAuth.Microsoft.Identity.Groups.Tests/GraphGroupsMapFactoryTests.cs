using Microsoft.Identity.Web;
using OidaAuth.Microsoft.Identity.Groups.Tests.Fakes;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OidaAuth.Microsoft.Identity.Groups.Tests
{
    public class GraphGroupsMapFactoryTests
    {
        [Fact]
        public void CtorThrowsIfIdentityOptionsAccessorIsNull()
        {
            Assert.Throws<ArgumentNullException>("identityOptionsAccessor", () => new GraphGroupsMapFactory(null!));
        }

        [Fact]
        public async Task CreateThrowsIfAuthenticationSchemeIsNull()
        {
            var factory = new GraphGroupsMapFactory(new FakeOptionsMonitor<MicrosoftIdentityOptions>());

            await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => factory.Create(null!));
        }
    }
}
