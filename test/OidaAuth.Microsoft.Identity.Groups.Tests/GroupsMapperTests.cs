using Microsoft.Identity.Web;
using OidaAuth.Microsoft.Identity.Groups.Tests.Fakes;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace OidaAuth.Microsoft.Identity.Groups.Tests
{
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
        public async Task AugmentPrincipalWithMappedRolesThrowsIfAuthenticationSchemeIsNullOrEmpty()
        {
            var mapper = GetGroupsMapper();

            await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => mapper.AugmentPrincipalWithMappedRoles(null!, new ClaimsPrincipal()));
            await Assert.ThrowsAsync<ArgumentException>("authenticationScheme", () => mapper.AugmentPrincipalWithMappedRoles(string.Empty, new ClaimsPrincipal()));
        }

        [Fact]
        public void AugmentPrincipalShouldDoNothingIfPrincipalIsNull()
        {
            var mapper = GetGroupsMapper();

            var result = mapper.AugmentPrincipalWithMappedRoles("Test", null!);

            Assert.Same(Task.CompletedTask, result);
        }

        [Fact]
        public async Task AugmentPrincipalWithMappedRolesShouldObtainRightOptionInstances()
        {
            var groupsMapFactory = new FakesGroupMapFactory();
            var obtainer = new GroupsMapObtainer(groupsMapFactory);
            var groupsMappingOptions = new FakeOptionsSnapshot<GroupsMappingOptions>();
            var identityOptions = new FakeOptionsSnapshot<MicrosoftIdentityOptions>(new()
            {
                TenantId = "Tenant1234"
            });
            var mapper = new GroupsMapper(obtainer, groupsMappingOptions, identityOptions);

            await mapper.AugmentPrincipalWithMappedRoles("Test", new());

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
}
