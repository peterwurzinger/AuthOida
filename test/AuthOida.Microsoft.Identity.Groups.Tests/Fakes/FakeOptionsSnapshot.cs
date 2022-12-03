using System;
using Microsoft.Extensions.Options;

namespace AuthOida.Microsoft.Identity.Groups.Tests.Fakes;

public class FakeOptionsSnapshot<T> : IOptionsSnapshot<T>
    where T : class, new()
{
    public T Value { get; }

    public FakeOptionsSnapshot(T value)
    {
        Value = value;
    }

    public FakeOptionsSnapshot(Action<T>? setup = null)
    {
        Value = new T();
        if (setup is not null)
            setup(Value);
    }

    public bool GetCalled { get; private set; }
    public string? LastName { get; private set; }
    public T Get(string? name)
    {
        GetCalled = true;
        LastName = name;
        return Value;
    }
}
