using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthOida.Microsoft.Identity.Groups.Tests.Fakes;
using Microsoft.Identity.Web;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests;

public class GroupsMapperTests
{
    [Fact]
    public void CtorThrowsIfParameterIsNull()
    {
        var obtainer = new GroupsMapObtainer(new FakesGroupMapFactory());
        var groupsMappingOptions = new FakeOptionsSnapshot<GroupsMappingOptions>();
        var identity = new FakeOptionsSnapshot<MicrosoftIdentityOptions>();

        Assert.Throws<ArgumentNullException>("groupsMapObtainer", () => new GroupsMapper(null!, groupsMappingOptions, identity));
        Assert.Throws<ArgumentNullException>("groupsMappingOptionsAccessor", () => new GroupsMapper(obtainer, null!, identity));
        Assert.Throws<ArgumentNullException>("identityOptionsAccessor", () => new GroupsMapper(obtainer, groupsMappingOptions, null!));
    }

    [Fact]
    public async Task EnrichPrincipalWithMappedRolesThrowsIfAuthenticationSchemeIsNullOrEmpty()
    {
        var mapper = GetGroupsMapper();

        await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => mapper.EnrichPrincipalWithMappedRoles(null!, new ClaimsPrincipal()));
        await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => mapper.EnrichPrincipalWithMappedRoles(string.Empty, new ClaimsPrincipal()));
    }

    [Fact]
    public void EnrichPrincipalShouldDoNothingIfPrincipalIsNull()
    {
        var mapper = GetGroupsMapper();

        var result = mapper.EnrichPrincipalWithMappedRoles("Test", null!);

        Assert.Same(Task.CompletedTask, result);
    }

    [Fact]
    public async Task EnrichPrincipalWithMappedRolesShouldObtainRightOptionInstances()
    {
        var groupsMapFactory = new FakesGroupMapFactory();
        var obtainer = new GroupsMapObtainer(groupsMapFactory);
        var groupsMappingOptions = new FakeOptionsSnapshot<GroupsMappingOptions>();
        var identityOptions = new FakeOptionsSnapshot<MicrosoftIdentityOptions>(new()
        {
            TenantId = "Tenant1234"
        });
        var mapper = new GroupsMapper(obtainer, groupsMappingOptions, identityOptions);

        await mapper.EnrichPrincipalWithMappedRoles("Test", new());

        Assert.True(groupsMappingOptions.GetCalled);
        Assert.Equal("Test", groupsMappingOptions.LastName);
        Assert.True(identityOptions.GetCalled);
        Assert.Equal("Test", identityOptions.LastName);
        Assert.Equal(1, groupsMapFactory.CallsPerAuthenticationScheme["Test"]);
    }

    private static GroupsMapper GetGroupsMapper()
    {
        var obtainer = new GroupsMapObtainer(new FakesGroupMapFactory());
        var groupsMappingOptions = new FakeOptionsSnapshot<GroupsMappingOptions>();
        var identity = new FakeOptionsSnapshot<MicrosoftIdentityOptions>();
        var mapper = new GroupsMapper(obtainer, groupsMappingOptions, identity);
        return mapper;
    }
}
