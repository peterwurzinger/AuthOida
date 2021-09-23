using Microsoft.Identity.Web;
using OidaAuth.Microsoft.Identity.Groups.Tests.Fakes;
using System;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace OidaAuth.Microsoft.Identity.Groups.Tests
{
    public class GroupsMappingTests
    {
        [Fact]
        public void CtorParameterNullThrows()
        {
            var groupsMap = new FakeGroupsMap();
            Assert.Throws<ArgumentNullException>("tenantId", () => new GroupsMapping(null!, "https://tokenclaimtype", "https://groupclaimtype", "authenticationtype", groupsMap));
            Assert.Throws<ArgumentNullException>("tokenGroupClaimType", () => new GroupsMapping("tenant1234", null!, "https://groupclaimtype", "authenticationtype", groupsMap));
            Assert.Throws<ArgumentNullException>("groupClaimType", () => new GroupsMapping("tenant1234", "https://tokenclaimtype", null!, "authenticationtype", groupsMap));
            Assert.Throws<ArgumentNullException>("authenticationType", () => new GroupsMapping("tenant1234", "https://tokenclaimtype", "https://groupclaimtype", null!, groupsMap));
            Assert.Throws<ArgumentNullException>("groupsMap", () => new GroupsMapping("tenant1234", "https://tokenclaimtype", "https://groupclaimtype", "authenticationtype", null!));
        }

        [Fact]
        public void PrepareShouldCreateNewInstance()
        {
            var identityOptions = new MicrosoftIdentityOptions
            {
                TenantId = "tenant1234"
            };
            var groupsMappingOptions = new GroupsMappingOptions
            {
                AuthenticationType = "authenticationtype",
                GroupClaimType = "https://groupclaimtype",
                TokenGroupClaimType = "https://tokenclaimtype"
            };

            var groupsMapping = GroupsMapping.Prepare(identityOptions, groupsMappingOptions, new FakeGroupsMap());

            Assert.NotNull(groupsMapping);
        }

        [Fact]
        public void PerformMappingOnShouldAddNewIdentityContainingClaimsWithGroupDisplayNameAsValue()
        {
            var principal = CreateClaimsPrincipal();
            var groupsMap = new FakeGroupsMap
            {
                GroupDisplayName = "TESTGROUPNAME"
            };

            var groupsMapping = new GroupsMapping("tenant1234", "tokenclaimtype", "groupclaimtype", "authenticationtype", groupsMap);
            groupsMapping.PerformMappingOn(principal);

            var identity = Assert.Single(principal.Identities, i => i.AuthenticationType == "authenticationtype");
            Assert.Equal("authenticationtype", identity.AuthenticationType);
            Assert.Single(identity.Claims, c => c.Type == "groupclaimtype" && c.Value == "TESTGROUPNAME");
        }

        [Fact]
        public void PerformMappingOnShouldNotAddIdentityIfNoGroupFound()
        {
            var principal = CreateClaimsPrincipal();
            var groupsMap = new FakeGroupsMap();

            var groupsMapping = new GroupsMapping("tenant1234", "tokenclaimtype", "groupclaimtype", "authenticationtype", groupsMap);
            groupsMapping.PerformMappingOn(principal);

            var identity = Assert.Single(principal.Identities);
            Assert.Equal(2, identity.Claims.Count());
        }

        private static ClaimsPrincipal CreateClaimsPrincipal()
        {
            var tenantClaim = new Claim(ClaimConstants.TenantId, "tenant1234");
            var groupIdClaim = new Claim("tokenclaimtype", "Id1234");
            var identity = new ClaimsIdentity();
            identity.AddClaim(tenantClaim);
            identity.AddClaim(groupIdClaim);

            return new ClaimsPrincipal(identity);
        }
    }
}
