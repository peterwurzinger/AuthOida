using System;
using System.Threading.Tasks;
using AuthOida.Microsoft.Identity.Groups.Tests.Fakes;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public class GroupsMapObtainerTests
{
    [Fact]
    public void CtorGroupsMapFactoryNullThrows()
    {
        Assert.Throws<ArgumentNullException>("groupsMapFactory", () => new GroupsMapObtainer(null!));
    }

    [Fact]
    public async Task GetOrCreateAuthenticationSchemeNullOrEmptyThrows()
    {
        var obtainer = new GroupsMapObtainer(new FakesGroupMapFactory());

        await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => obtainer.GetOrCreate(null!));
        await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => obtainer.GetOrCreate(string.Empty));
    }

    [Fact]
    public async Task GetOrCreateShouldCreateInstancesOnlyOnceForSameAuthenticationScheme()
    {
        var factory = new FakesGroupMapFactory();
        var obtainer = new GroupsMapObtainer(factory);

        var first = await obtainer.GetOrCreate("TestScheme");
        var second = await obtainer.GetOrCreate("TestScheme");

        Assert.Same(first, second);
        Assert.Equal(1, factory.CallsPerAuthenticationScheme["TestScheme"]);
    }

    [Fact]
    public async Task GetOrCreateShouldCreateDifferentInstancesForDifferentAuthenticationSchemes()
    {
        var factory = new FakesGroupMapFactory();
        var obtainer = new GroupsMapObtainer(factory);

        var that = await obtainer.GetOrCreate("TestScheme");
        var other = await obtainer.GetOrCreate("AnotherScheme");

        Assert.NotNull(other);
        Assert.NotSame(that, other);
        Assert.Equal(1, factory.CallsPerAuthenticationScheme["AnotherScheme"]);
    }
}
