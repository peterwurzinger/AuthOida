using System.Diagnostics.CodeAnalysis;

namespace AuthOida.Microsoft.Identity.Groups
{
    /// <summary>
    /// Provides a mechanism to perform a lookup of a groups display name given its Id.
    /// </summary>
    public interface IGroupsMap
    {
        /// <summary>
        /// Gets the groups display name that is associated with the specified <paramref name="groupId"/>.
        /// </summary>
        /// <param name="groupId">The group Id to locate.</param>
        /// <param name="groupDisplayName">When this method returns, the display name associated with the specified Id, if the Id is found; otherwise null. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the map contains a group that has the specified Id; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="groupId"/> is null</exception>
        bool TryGetValue(string groupId, [MaybeNullWhen(false)] out string groupDisplayName);
    }
}
