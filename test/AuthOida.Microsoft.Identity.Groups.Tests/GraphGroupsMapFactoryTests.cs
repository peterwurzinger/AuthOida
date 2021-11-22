using System;
using System.Threading.Tasks;
using AuthOida.Microsoft.Identity.Groups.Tests.Fakes;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public class GraphGroupsMapFactoryTests
{
    [Fact]
    public void CtorThrowsIfIdentityOptionsAccessorIsNull()
    {
        Assert.Throws<ArgumentNullException>("identityOptionsAccessor", () => new GraphGroupsMapFactory(null!));
    }

    [Fact]
    public async Task CreateThrowsIfAuthenticationSchemeIsNullOrEmpty()
    {
        var factory = new GraphGroupsMapFactory(new FakeOptionsMonitor<MicrosoftIdentityOptions>());

        await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => factory.Create(null!));
        await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => factory.Create(string.Empty));
    }
}
