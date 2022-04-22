using System.Diagnostics.CodeAnalysis;

namespace AuthOida.Microsoft.Identity.Groups.Tests.Fakes;

public class FakeGroupsMap : IGroupsMap
{
    public string? GroupDisplayName { get; set; }
    public bool TryGetValueCalled { get; private set; }

    public FakeGroupsMap()
    {
        GroupDisplayName = "FakeGroupName";
    }

    public bool TryGetValue(string groupId, [MaybeNullWhen(false)] out string groupDisplayName)
    {
        TryGetValueCalled = true;
        if (GroupDisplayName is not null)
        {
            groupDisplayName = GroupDisplayName;
            return true;
        }

        groupDisplayName = null;
        return false;
    }
}
