using System.Diagnostics.Contracts;

// ReSharper disable InconsistentNaming

namespace Kehlet.Functional;

public static partial class Prelude
{
    [Pure]
    public static Option<TValue> some<TValue>(TValue value)
        where TValue : notnull =>
        new(value);

    [Pure]
    public static Option<(TValue1, TValue2)> some<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        new((value1, value2));

    public static NoneOption none =>
        default;

    [Pure]
    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Option<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsSome
        select value.value;

    [Pure]
    public static IEnumerable<TValue> Filter<TValue>(this IEnumerable<Option<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsSome
        select value.value;
}
