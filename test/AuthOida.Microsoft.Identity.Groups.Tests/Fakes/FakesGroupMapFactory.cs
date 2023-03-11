using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthOida.Microsoft.Identity.Groups.Tests.Fakes;

public class FakesGroupMapFactory : IGroupsMapFactory
{
    private readonly Dictionary<string, int> _callsPerAuthenticationScheme;
    public IReadOnlyDictionary<string, int> CallsPerAuthenticationScheme => _callsPerAuthenticationScheme;

    public FakesGroupMapFactory()
    {
        _callsPerAuthenticationScheme = new Dictionary<string, int>();
    }

    public Task<IGroupsMap> Create(string authenticationScheme, CancellationToken cancellationToken = default)
    {
        if (_callsPerAuthenticationScheme.TryGetValue(authenticationScheme, out var value))
            _callsPerAuthenticationScheme[authenticationScheme]++;

        _callsPerAuthenticationScheme[authenticationScheme] = 1;

        return Task.FromResult((IGroupsMap)new FakeGroupsMap());
    }
}
