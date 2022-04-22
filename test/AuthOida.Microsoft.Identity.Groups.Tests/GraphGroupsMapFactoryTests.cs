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

    private const string Message = "'authenticationScheme' cannot be null or empty. (Parameter 'authenticationScheme')";

    [Fact]
    public async Task CreateThrowsIfAuthenticationSchemeIsNull()
    {
        var factory = new GraphGroupsMapFactory(new FakeOptionsMonitor<MicrosoftIdentityOptions>());

        var exception = await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => factory.Create(null!));
        Assert.Equal(Message, exception.Message);
    }

    [Fact]
    public async Task CreateThrowsIfAuthenticationSchemeIsEmpty()
    {
        var factory = new GraphGroupsMapFactory(new FakeOptionsMonitor<MicrosoftIdentityOptions>());

        var exception = await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => factory.Create(string.Empty));
        Assert.Equal(Message, exception.Message);
    }
}
