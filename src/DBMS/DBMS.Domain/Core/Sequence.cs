using System;

namespace DBMS.Domain.Core;

public class Sequence
{
    public string Name { get; private set; }
    public long CurrentValue { get; private set; }
    public long Increment { get; private set; }

    public Sequence(string name, long startValue = 0, long increment = 1)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Sequence name cannot be empty", nameof(name));
        Name = name;
        CurrentValue = startValue;
        Increment = increment;
    }

    public long NextValue()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
