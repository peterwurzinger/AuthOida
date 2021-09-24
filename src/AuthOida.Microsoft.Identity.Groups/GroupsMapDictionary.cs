using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AuthOida.Microsoft.Identity.Groups
{
    /// <summary>
    /// Implements <see cref="IGroupsMap"/> by performing a lookup in a dictionary passed by an <see cref="IGroupsMapFactory"/>.
    /// </summary>
    public sealed class GroupsMapDictionary : IGroupsMap, IReadOnlyDictionary<string, string>
    {
        private readonly Dictionary<string, string> _backingDictionary;

        /// <summary>
        /// Creates a new <see cref="GroupsMapDictionary"/> by copying entries from an already populated dictionary.
        /// </summary>
        /// <param name="backingDictionary">The dictionary to copy the state from</param>
        /// <exception cref="ArgumentNullException">When <paramref name="backingDictionary"/> is null</exception>
        public GroupsMapDictionary(IReadOnlyDictionary<string, string> backingDictionary)
        {
            if (backingDictionary is null)
                throw new ArgumentNullException(nameof(backingDictionary));

            _backingDictionary = new Dictionary<string, string>(backingDictionary);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string groupId, [MaybeNullWhen(false)] out string groupDisplayName)
            => _backingDictionary.TryGetValue(groupId, out groupDisplayName);

        /// <inheritdoc/>
        public string this[string key] => _backingDictionary[key];

        /// <inheritdoc/>
        public IEnumerable<string> Keys => _backingDictionary.Keys;

        /// <inheritdoc/>
        public IEnumerable<string> Values => _backingDictionary.Values;

        /// <inheritdoc/>
        public int Count => _backingDictionary.Count;

        /// <inheritdoc/>
        public bool ContainsKey(string key) => _backingDictionary.ContainsKey(key);

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _backingDictionary.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
