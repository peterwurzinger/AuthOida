using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace AuthOida.Microsoft.Identity.Groups;

internal sealed class GroupsMapObtainer
{
    private readonly IGroupsMapFactory _groupsMapFactory;
    private readonly IMemoryCache _memoryCache;

    public GroupsMapObtainer(IGroupsMapFactory groupsMapFactory, IMemoryCache memoryCache)
    {
        _groupsMapFactory = groupsMapFactory ?? throw new ArgumentNullException(nameof(groupsMapFactory));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    internal Task<IGroupsMap> GetOrCreate(string authenticationScheme, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"authOida:{authenticationScheme}:groupsMap";
        return _memoryCache.GetOrCreateAsync(cacheKey, _ => _groupsMapFactory.Create(authenticationScheme, cancellationToken))!;
    }
}
