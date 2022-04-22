using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace AuthOida.Microsoft.Identity.Groups;

internal sealed class GroupsMapper
{
    private readonly GroupsMapObtainer _groupsMapsObtainer;
    private readonly IOptionsSnapshot<MicrosoftIdentityOptions> _identityOptionsAccessor;
    private readonly IOptionsSnapshot<GroupsMappingOptions> _groupsMappingOptionsAccessor;

    internal GroupsMapper(GroupsMapObtainer groupsMapObtainer, IOptionsSnapshot<GroupsMappingOptions> groupsMappingOptionsAccessor, IOptionsSnapshot<MicrosoftIdentityOptions> identityOptionsAccessor)
    {
        _groupsMapsObtainer = groupsMapObtainer ?? throw new ArgumentNullException(nameof(groupsMapObtainer));
        _groupsMappingOptionsAccessor = groupsMappingOptionsAccessor ?? throw new ArgumentNullException(nameof(groupsMappingOptionsAccessor));
        _identityOptionsAccessor = identityOptionsAccessor ?? throw new ArgumentNullException(nameof(identityOptionsAccessor));
    }

    internal Task EnrichPrincipalWithMappedRoles(string authenticationScheme, ClaimsPrincipal? claimsPrincipal, CancellationToken cancellationToken = default)
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

        var groupsMapping = GroupsMapping.Create(identityOptions, groupsMappingOptions, groupsMap);

        var groupsIdentity = groupsMapping.PerformMappingOn(claimsPrincipal);
        if (groupsIdentity is not null)
            claimsPrincipal.AddIdentity(groupsIdentity);
    }
}
