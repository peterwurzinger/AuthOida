using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthOida.Microsoft.Identity.Groups;

internal sealed class GroupsMapObtainer
{
    private readonly IGroupsMapFactory _groupsMapFactory;
    private readonly Dictionary<string, IGroupsMap> _groupMaps;

    internal GroupsMapObtainer(IGroupsMapFactory groupsMapFactory)
    {
        _groupsMapFactory = groupsMapFactory ?? throw new ArgumentNullException(nameof(groupsMapFactory));
        _groupMaps = new Dictionary<string, IGroupsMap>();
    }

    internal async Task<IGroupsMap> GetOrCreate(string authenticationScheme, CancellationToken cancellationToken = default)
    {
        await EnsureCreated(authenticationScheme, cancellationToken).ConfigureAwait(false);

        return _groupMaps[authenticationScheme];
    }

    private async Task EnsureCreated(string authenticationScheme, CancellationToken cancellationToken = default)
    {
        var isContained = _groupMaps.ContainsKey(authenticationScheme);
        if (isContained)
            return;

        var instance = await _groupsMapFactory.Create(authenticationScheme, cancellationToken).ConfigureAwait(false);
        _groupMaps[authenticationScheme] = instance;
    }
}
