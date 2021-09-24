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

        public GroupsMapDictionary(IReadOnlyDictionary<string, string> backingDictionary)
        {
            if (backingDictionary is null)
                throw new ArgumentNullException(nameof(backingDictionary));

            _backingDictionary = new Dictionary<string, string>(backingDictionary);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string groupId, [MaybeNullWhen(false)] out string groupDisplayName)
            => _backingDictionary.TryGetValue(groupId, out groupDisplayName);

        public string this[string key] => _backingDictionary[key];
        public IEnumerable<string> Keys => _backingDictionary.Keys;
        public IEnumerable<string> Values => _backingDictionary.Values;
        public int Count => _backingDictionary.Count;
        public bool ContainsKey(string key) => _backingDictionary.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _backingDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
