﻿using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OidaAuth.Microsoft.Identity.Groups
{
    /// <summary>
    /// Maps the group Ids provided in a given <see cref="ClaimsPrincipal"/> to new <see cref="Claim"/>s by performing a
    /// lookup of the display name in an <see cref="IGroupsMap"/>.
    /// </summary>
    public sealed class GroupsMapper
    {
        private readonly GroupsMapObtainer _groupsMapsObtainer;
        private readonly IOptionsSnapshot<MicrosoftIdentityOptions> _identityOptionsAccessor;
        private readonly IOptionsSnapshot<GroupsMappingOptions> _groupsMappingOptionsAccessor;

        /// <summary>
        /// Creates a new <see cref="GroupsMapper"/>
        /// </summary>
        /// <param name="groupsMapObtainer">The <see cref="GroupsMapObtainer#"/> to access the <see cref="IGroupsMap"/> for a given authenticatino scheme</param>
        /// <param name="groupMappingOptionsAccessor">Accessor for <see cref="GroupsMappingOptions"/> for a given authentication scheme</param>
        /// <param name="identityOptionsAccessor">Accessor for <see cref="MicrosoftIdentityOptions"/> for a given authentication scheme</param>
        /// <exception cref="ArgumentNullException"><paramref name="groupsMapObtainer"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="groupMappingOptionsAccessor"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityOptionsAccessor"/> is null</exception>
        public GroupsMapper(GroupsMapObtainer groupsMapObtainer, IOptionsSnapshot<GroupsMappingOptions> groupMappingOptionsAccessor, IOptionsSnapshot<MicrosoftIdentityOptions> identityOptionsAccessor)
        {
            _groupsMapsObtainer = groupsMapObtainer ?? throw new ArgumentNullException(nameof(groupsMapObtainer));
            _groupsMappingOptionsAccessor = groupMappingOptionsAccessor ?? throw new ArgumentNullException(nameof(groupMappingOptionsAccessor));
            _identityOptionsAccessor = identityOptionsAccessor ?? throw new ArgumentNullException(nameof(identityOptionsAccessor));
        }

        /// <summary>
        /// Adds a new <see cref="ClaimsIdentity"/> to the given <see cref="ClaimsPrincipal"/>, that is made out of 
        /// display name <see cref="Claim"/>s representing the result of the lookup in <see cref="IGroupsMap"/>.
        /// Only claims, whichs type match <see cref="GroupsMappingOptions.TokenGroupClaimType"/> and were originally issued by an issuer in
        /// <see cref="GroupsMappingOptions.ValidIssuers"/> are candidates for a lookup.
        /// </summary>
        /// <param name="authenticationScheme">The authentication scheme for which the group Ids should be mapped</param>
        /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> to augment with a new identity, that contains the mapped claims</param>
        /// <exception cref="ArgumentException">If <paramref name="authenticationScheme"/> is null or empty</exception>
        public Task AugmentPrincipalWithMappedRoles(string authenticationScheme, ClaimsPrincipal? claimsPrincipal, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(authenticationScheme))
                throw new ArgumentException($"'{nameof(authenticationScheme)}' cannot be null or empty.", nameof(authenticationScheme));

            if (claimsPrincipal is null)
                return Task.CompletedTask;

            return AugmentPrincipalWithMappedRolesInternal(authenticationScheme, claimsPrincipal, cancellationToken);
        }

        private async Task AugmentPrincipalWithMappedRolesInternal(string authenticationScheme, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default)
        {
            var groupsMappingOptions = _groupsMappingOptionsAccessor.Get(authenticationScheme);
            var groupClaims = GetGroupClaims(authenticationScheme, claimsPrincipal, groupsMappingOptions.TokenGroupClaimType);

            var groupsMap = await _groupsMapsObtainer.GetOrCreate(authenticationScheme, cancellationToken).ConfigureAwait(false);
            var mappedRoles = new List<Claim>();
            foreach (var groupClaim in groupClaims)
            {
                Map(groupsMap, groupsMappingOptions.GroupClaimType, mappedRoles, groupClaim);
            }

            if (mappedRoles.Any())
                AugmentPrincipal(groupsMappingOptions.AuthenticationType, claimsPrincipal, mappedRoles);
        }

        private Claim[] GetGroupClaims(string authenticationScheme, ClaimsPrincipal claimsPrincipal, string tokenGroupClaimType)
        {
            var identityOptions = _identityOptionsAccessor.Get(authenticationScheme);

            var groupClaims = from identity in claimsPrincipal.Identities
                              let tenantIdClaim = identity.FindFirst(ClaimConstants.TenantId)
                              where tenantIdClaim is not null
                              where tenantIdClaim?.Value == identityOptions.TenantId

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

        private static void AugmentPrincipal(string authenticationType, ClaimsPrincipal claimsPrincipal, List<Claim> mappedRoles)
        {
            var mappedRolesIdentity = new ClaimsIdentity(authenticationType);
            mappedRolesIdentity.AddClaims(mappedRoles);

            claimsPrincipal.AddIdentity(mappedRolesIdentity);
        }
    }
}
