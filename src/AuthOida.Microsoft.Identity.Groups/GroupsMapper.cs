using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AuthOida.Microsoft.Identity.Groups
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
        /// <param name="groupsMapObtainer">The <see cref="GroupsMapObtainer"/> to access the <see cref="IGroupsMap"/> for a given authentication scheme</param>
        /// <param name="groupsMappingOptionsAccessor">Accessor for <see cref="GroupsMappingOptions"/> for a given authentication scheme</param>
        /// <param name="identityOptionsAccessor">Accessor for <see cref="MicrosoftIdentityOptions"/> for a given authentication scheme</param>
        /// <exception cref="ArgumentNullException"><paramref name="groupsMapObtainer"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="groupsMappingOptionsAccessor"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityOptionsAccessor"/> is null</exception>
        public GroupsMapper(GroupsMapObtainer groupsMapObtainer, IOptionsSnapshot<GroupsMappingOptions> groupsMappingOptionsAccessor, IOptionsSnapshot<MicrosoftIdentityOptions> identityOptionsAccessor)
        {
            _groupsMapsObtainer = groupsMapObtainer ?? throw new ArgumentNullException(nameof(groupsMapObtainer));
            _groupsMappingOptionsAccessor = groupsMappingOptionsAccessor ?? throw new ArgumentNullException(nameof(groupsMappingOptionsAccessor));
            _identityOptionsAccessor = identityOptionsAccessor ?? throw new ArgumentNullException(nameof(identityOptionsAccessor));
        }

        /// <summary>
        /// Adds a new <see cref="ClaimsIdentity"/> to the given <see cref="ClaimsPrincipal"/>, that is made out of 
        /// display name <see cref="Claim"/>s representing the result of the lookup in <see cref="IGroupsMap"/>.
        /// Only claims, whichs type match <see cref="GroupsMappingOptions.TokenGroupClaimType"/> and were originally issued by the tenant in
        /// <see cref="MicrosoftIdentityOptions.TenantId"/> are candidates for a lookup.
        /// </summary>
        /// <param name="authenticationScheme">The authentication scheme for which the group Ids should be mapped</param>
        /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> to enrich with a new identity, that contains the mapped claims</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the enrichment of the principal</param>
        /// <exception cref="ArgumentException">If <paramref name="authenticationScheme"/> is null or empty</exception>
        public Task EnrichPrincipalWithMappedRoles(string authenticationScheme, ClaimsPrincipal? claimsPrincipal, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(authenticationScheme))
                throw new ArgumentException($"'{nameof(authenticationScheme)}' cannot be null or empty.", nameof(authenticationScheme));

            if (claimsPrincipal is null)
                return Task.CompletedTask;

            return EnrichPrincipalWithMappedRolesInternal(authenticationScheme, claimsPrincipal, cancellationToken);
        }

        private async Task EnrichPrincipalWithMappedRolesInternal(string authenticationScheme, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            var identityOptions = _identityOptionsAccessor.Get(authenticationScheme);
            var groupsMappingOptions = _groupsMappingOptionsAccessor.Get(authenticationScheme);
            var groupsMap = await _groupsMapsObtainer.GetOrCreate(authenticationScheme, cancellationToken).ConfigureAwait(false);

            var groupsMapping = GroupsMapping.Prepare(identityOptions, groupsMappingOptions, groupsMap);

            groupsMapping.PerformMappingOn(claimsPrincipal);
        }
    }
}
