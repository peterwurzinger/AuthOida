using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthOida.Microsoft.Identity.Groups
{
    /// <summary>
    /// A holder class to keep track of <see cref="IGroupsMap"/>s per configured authentication scheme.
    /// </summary>
    public sealed class GroupsMapObtainer
    {
        private readonly IGroupsMapFactory _groupsMapFactory;
        private readonly Dictionary<string, IGroupsMap> _groupMaps;

        /// <summary>
        /// Creates a <see cref="GroupsMapObtainer"/>
        /// </summary>
        /// <param name="groupsMapFactory">The factory to create an instance of <see cref="IGroupsMap"/>
        /// for a given authentication scheme</param>
        /// <exception cref="ArgumentNullException">when <paramref name="groupsMapFactory"/> is null</exception>
        public GroupsMapObtainer(IGroupsMapFactory groupsMapFactory)
        {
            _groupsMapFactory = groupsMapFactory ?? throw new ArgumentNullException(nameof(groupsMapFactory));
            _groupMaps = new Dictionary<string, IGroupsMap>();
        }

        /// <summary>
        /// Gets the <see cref="IGroupsMap"/> for a given <paramref name="authenticationScheme"/> and creates
        /// one, if not yet created.
        /// </summary>
        /// <param name="authenticationScheme">The authentication scheme for which to obtain the
        /// corresponding <see cref="IGroupsMap"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation</param>
        /// <returns>An instance of <see cref="IGroupsMap"/> for the given <paramref name="authenticationScheme"/></returns>
        /// <exception cref="ArgumentException">When <paramref name="authenticationScheme"/> is null or emtpy</exception>
        public Task<IGroupsMap> GetOrCreate(string authenticationScheme, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(authenticationScheme))
                throw new ArgumentException($"'{nameof(authenticationScheme)}' cannot be null or empty.", nameof(authenticationScheme));

            return GetOrCreateInternal(authenticationScheme, cancellationToken);
        }

        private async Task<IGroupsMap> GetOrCreateInternal(string authenticationScheme, CancellationToken cancellationToken = default)
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
}
