using System.Diagnostics.Contracts;

namespace Kehlet.Functional;

public readonly record struct NoneOption
{
    [Pure]
    public Option<TValue> ToOption<TValue>()
        where TValue : notnull => none;
}
