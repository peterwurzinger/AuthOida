using System.Security.Claims;

namespace AuthOida.Microsoft.Identity.Groups;

/// <summary>
/// Options to configure the behavior for mapping group Ids to their display names.
/// </summary>
public class GroupsMappingOptions
{
    /// <summary>
    /// The type of the group-claim from the token. Defaults to <c>"groups"</c>.
    /// </summary>
    public string TokenGroupClaimType { get; set; }

    /// <summary>
    /// The authentication type for the created <see cref="ClaimsIdentity"/> holding the group display names.
    /// Defaults to <c>"MappedGroups"</c>.
    /// </summary>
    public string AuthenticationType { get; set; }

    /// <summary>
    /// The claim type for the created <see cref="Claim"/>s. Defaults to <see cref="ClaimTypes.Role"/>.
    /// </summary>
    public string GroupClaimType { get; set; }

    /// <summary>
    /// Creates new GroupsMappingOptions
    /// </summary>
    public GroupsMappingOptions()
    {
        TokenGroupClaimType = "groups";
        AuthenticationType = "MappedGroups";
        GroupClaimType = ClaimTypes.Role;
    }
}
