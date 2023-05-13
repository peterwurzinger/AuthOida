using System;
using System.Threading.Tasks;
using AuthOida.Microsoft.Identity.Groups.Tests.Fakes;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public sealed class GroupsMapObtainerTests : IDisposable
{
    private readonly MemoryCache _memoryCache;

    public GroupsMapObtainerTests()
    {
        _memoryCache = new MemoryCache(new FakeOptionsSnapshot<MemoryCacheOptions>());
    }

    [Fact]
    public void CtorGroupsMapFactoryNullThrows()
    {
        Assert.Throws<ArgumentNullException>("groupsMapFactory", () => new GroupsMapObtainer(null!, new MemoryCache(new FakeOptionsSnapshot<MemoryCacheOptions>())));
    }

    [Fact]
    public void CtorMemoryCacheNullThrows()
    {
        Assert.Throws<ArgumentNullException>("memoryCache", () => new GroupsMapObtainer(new FakesGroupMapFactory(), null!));
    }

    [Fact]
    public async Task GetOrCreateShouldCreateInstancesOnlyOnceForSameAuthenticationScheme()
    {
        var factory = new FakesGroupMapFactory();
        var obtainer = new GroupsMapObtainer(factory, _memoryCache);

        var first = await obtainer.GetOrCreate("TestScheme");
        var second = await obtainer.GetOrCreate("TestScheme");

        Assert.Same(first, second);
        Assert.Equal(1, factory.CallsPerAuthenticationScheme["TestScheme"]);
    }

    [Fact]
    public async Task GetOrCreateShouldCreateDifferentInstancesForDifferentAuthenticationSchemes()
    {
        var factory = new FakesGroupMapFactory();
        var obtainer = new GroupsMapObtainer(factory, _memoryCache);

        var that = await obtainer.GetOrCreate("TestScheme");
        var other = await obtainer.GetOrCreate("AnotherScheme");

        Assert.NotNull(other);
        Assert.NotSame(that, other);
        Assert.Equal(1, factory.CallsPerAuthenticationScheme["AnotherScheme"]);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}
