﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace AuthOida.Microsoft.Identity.Groups;

/// <summary>
/// Creates instances of <see cref="GroupsMapDictionary"/> by querying the Microsoft Graph API
/// </summary>
public sealed class GraphGroupsMapFactory : IGroupsMapFactory
{
    private const string ReadGroupsScope = "Groups.Read.All";

    private readonly IOptionsMonitor<MicrosoftIdentityOptions> _identityOptionsAccessor;

    /// <summary>
    /// Constructs a new <see cref="GraphGroupsMapFactory"/>.
    /// </summary>
    /// <param name="identityOptionsAccessor">The <see cref="MicrosoftIdentityOptions"/> for authenticating the application</param>
    /// <exception cref="ArgumentNullException">When <paramref name="identityOptionsAccessor"/> is null</exception>
    public GraphGroupsMapFactory(IOptionsMonitor<MicrosoftIdentityOptions> identityOptionsAccessor)
    {
        _identityOptionsAccessor = identityOptionsAccessor ?? throw new ArgumentNullException(nameof(identityOptionsAccessor));
    }

    /// <summary>
    /// Creates a <see cref="IGroupsMap"/> by querying the security groups of the configured Azure Active Directory instance via Microsoft Graph.
    /// The application needs to have <b>application scoped</b> permissions to read all groups (<c>"Groups.Read.All"</c>).
    /// See <see href="https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#permission-types"/> for details.
    /// </summary>
    /// <exception cref="ArgumentException">When <paramref name="authenticationScheme"/> is null or empty</exception>
    /// <exception cref="InvalidOperationException">When there is no client secret and no client certificate
    /// configured for the <paramref name="authenticationScheme"/></exception>
    /// <inheritdoc/>
    public Task<IGroupsMap> Create(string authenticationScheme, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(authenticationScheme))
            throw new ArgumentException($"'{nameof(authenticationScheme)}' cannot be null or empty.", nameof(authenticationScheme));

        return CreateInternal(authenticationScheme, cancellationToken);
    }

    private async Task<IGroupsMap> CreateInternal(string authenticationScheme, CancellationToken cancellationToken = default)
    {
        var client = CreateClient(authenticationScheme);
        var page = await client.Groups.Request()
                                      .WithScopes(ReadGroupsScope)
                                      .WithAppOnly()
                                      .Select(g => new
                                      {
                                          g.Id,
                                          g.DisplayName
                                      })
                                      .GetAsync(cancellationToken)
                                      .ConfigureAwait(false);

        var intermediateDictionary = new ConcurrentDictionary<string, string>();
        var pageIterator = PageIterator<Group>.CreatePageIterator(client, page, group =>
        {
            intermediateDictionary.GetOrAdd(group.Id, group.DisplayName);
            return true;
        });

        await pageIterator.IterateAsync(cancellationToken).ConfigureAwait(false);

        return new GroupsMapDictionary(intermediateDictionary);
    }

    private GraphServiceClient CreateClient(string authenticationScheme)
    {
        var identityOptions = _identityOptionsAccessor.Get(authenticationScheme);

        var credential = TokenCredentialConversion.FromIdentityOptions(identityOptions);

        return new GraphServiceClient(credential);
    }
}
