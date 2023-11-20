// ReSharper disable InconsistentNaming

namespace Kehlet.Functional;

public readonly struct Option<TValue>(TValue value)
{
    internal readonly TValue value = value;

    public bool IsSome { get; } = true;
    public bool IsNone => !IsSome;

    public TResult Match<TResult>(Func<TValue, TResult> some, Func<TResult> none) =>
        IsSome
            ? some(value)
            : none();

    public Option<TValue> Where(Func<TValue, bool> predicate) =>
        IsSome && predicate(value)
            ? this
            : none;

    public Option<TResult> Select<TResult>(Func<TValue, TResult> selector) =>
        IsSome
            ? some(selector(value))
            : none;

    public Option<TResult> Select<TResult>(Func<TValue, Option<TResult>> selector) =>
        IsSome
            ? selector(value)
            : none;

    public Option<TResult> SelectMany<TValue2, TResult>(Func<TValue, Option<TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
    {
        if (IsNone)
        {
            return none;
        }

        var inner = selector(value);

        if (inner.IsNone)
        {
            return none;
        }

        return some(resultSelector(value, inner.value));
    }

    public Option<TResult> SelectMany<TValue2, TResult>(Func<TValue, Option<TValue2>> selector, Func<TValue, TValue2, Option<TResult>> resultSelector)
    {
        if (IsNone)
        {
            return none;
        }

        var inner = selector(value);

        if (inner.IsNone)
        {
            return none;
        }

        return resultSelector(value, inner.value);
    }

    public Result<TValue> ToResult(string error) =>
        IsSome
            ? ok(value)
            : Prelude.error(error);

    public override string ToString() =>
        IsSome
            ? $"some {value}"
            : "none";

    public static implicit operator Option<TValue>(NoneOption _) => default;
    public static implicit operator Option<TValue>(OptionUnion<TValue>.Some option) => new(option.Value);
    public static implicit operator Option<TValue>(OptionUnion<TValue>.None _) => default;
    public static Option<TValue> operator |(Option<TValue> lhs, Option<TValue> rhs) => lhs.IsSome ? lhs : rhs;
}

public readonly record struct NoneOption;

public static partial class Prelude
{
    public static Option<TValue> some<TValue>(TValue value) =>
        new(value);

    public static Option<(TValue1, TValue2)> some<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        new((value1, value2));

    public static NoneOption none =>
        default;

    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Option<TValue>> values) =>
        from value in values
        where value.IsSome
        select value.value;
}
