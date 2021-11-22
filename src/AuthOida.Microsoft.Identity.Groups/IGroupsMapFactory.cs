using System.Threading;
using System.Threading.Tasks;

namespace AuthOida.Microsoft.Identity.Groups;

/// <summary>
/// A factory to create instances of <see cref="IGroupsMap"/>
/// </summary>
public interface IGroupsMapFactory
{
    /// <summary>
    /// Creates a new instance of an <see cref="IGroupsMap"/>
    /// </summary>
    /// <param name="authenticationScheme">The authentication scheme to create the <see cref="IGroupsMap"/> for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the creation</param>
    /// <returns>An instance of <see cref="IGroupsMap"/> based on the lookup mechanism used</returns>
    Task<IGroupsMap> Create(string authenticationScheme, CancellationToken cancellationToken = default);
}
