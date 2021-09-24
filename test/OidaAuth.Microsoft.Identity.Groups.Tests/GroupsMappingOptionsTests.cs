using System.Security.Claims;
using Xunit;

namespace OidaAuth.Microsoft.Identity.Groups.Tests
{
    public class GroupsMappingOptionsTests
    {
        [Fact]
        public void CtorShouldSetDefaultValues()
        {
            var options = new GroupsMappingOptions();

            Assert.Equal("groups", options.TokenGroupClaimType);
            Assert.Equal("MappedGroups", options.AuthenticationType);
            Assert.Equal(ClaimTypes.Role, options.GroupClaimType);
        }
    }
}
