using OidaAuth.Microsoft.Identity.Groups;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OidaAuth.Microsoft.Identity.Groups.Tests.Fakes
{
    public class FakeGroupsMap : IGroupsMap
    {
        public bool TryGetValue(string groupId, [MaybeNullWhen(false)] out string groupDisplayName)
        {
            throw new NotImplementedException();
        }
    }
}
