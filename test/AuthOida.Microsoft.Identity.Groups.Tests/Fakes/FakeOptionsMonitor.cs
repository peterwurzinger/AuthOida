using Microsoft.Extensions.Options;
using System;

namespace AuthOida.Microsoft.Identity.Groups.Tests.Fakes
{
    public class FakeOptionsMonitor<T> : FakeOptionsSnapshot<T>, IOptionsMonitor<T>
        where T : class, new()
    {
        public T CurrentValue => base.Value;

        public FakeOptionsMonitor()
        {
        }

        public IDisposable OnChange(Action<T, string> listener)
        {
            throw new NotImplementedException();
        }
    }
}
