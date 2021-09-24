using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AuthOida.Microsoft.Identity.Groups
{
    internal class GroupsMapping
    {
        private readonly string _tenantId;
        private readonly string _tokenGroupClaimType;
        private readonly string _groupClaimType;
        private readonly string _authenticationType;
        private readonly IGroupsMap _groupsMap;

        internal GroupsMapping(string tenantId, string tokenGroupClaimType, string groupClaimType, string authenticationType, IGroupsMap groupsMap)
        {
            _tenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            _tokenGroupClaimType = tokenGroupClaimType ?? throw new ArgumentNullException(nameof(tokenGroupClaimType));
            _groupClaimType = groupClaimType ?? throw new ArgumentNullException(nameof(groupClaimType));
            _authenticationType = authenticationType ?? throw new ArgumentNullException(nameof(authenticationType));
            _groupsMap = groupsMap ?? throw new ArgumentNullException(nameof(groupsMap));
        }

        internal static GroupsMapping Prepare(MicrosoftIdentityOptions identityOptions, GroupsMappingOptions groupsMappingOptions, IGroupsMap groupsMap)
        {
            return new GroupsMapping(identityOptions.TenantId!, groupsMappingOptions.TokenGroupClaimType, groupsMappingOptions.GroupClaimType, groupsMappingOptions.AuthenticationType, groupsMap);
        }

        internal void PerformMappingOn(ClaimsPrincipal claimsPrincipal)
        {
            var groupClaims = GetGroupClaims(claimsPrincipal, _tenantId, _tokenGroupClaimType);

            var mappedRoles = new List<Claim>();
            foreach (var groupClaim in groupClaims)
            {
                Map(_groupsMap, _groupClaimType, mappedRoles, groupClaim);
            }

            if (mappedRoles.Any())
                AddClaimsAsIdentity(_authenticationType, claimsPrincipal, mappedRoles);
        }

        private static Claim[] GetGroupClaims(ClaimsPrincipal claimsPrincipal, string tenantId, string tokenGroupClaimType)
        {
            var groupClaims = from identity in claimsPrincipal.Identities
                              let tenantIdClaim = identity.FindFirst(ClaimConstants.TenantId)
                              where tenantIdClaim is not null
                              where tenantIdClaim.Value == tenantId

                              from claim in identity.Claims
                              where claim.Type == tokenGroupClaimType
                              select claim;

            return groupClaims.ToArray();
        }

        private static void Map(IGroupsMap groupsMap, string groupClaimType, ICollection<Claim> mappedRoles, Claim msalGroupClaim)
        {
            var groupExists = groupsMap.TryGetValue(msalGroupClaim.Value, out var groupDisplayName);
            if (groupExists)
            {
                var claim = new Claim(groupClaimType, groupDisplayName!, ClaimValueTypes.String);
                mappedRoles.Add(claim);
            }
        }

        private static void AddClaimsAsIdentity(string authenticationType, ClaimsPrincipal claimsPrincipal, List<Claim> mappedRoles)
        {
            var mappedRolesIdentity = new ClaimsIdentity(authenticationType);
            mappedRolesIdentity.AddClaims(mappedRoles);

            claimsPrincipal.AddIdentity(mappedRolesIdentity);
        }
    }
}
