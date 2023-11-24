// ReSharper disable InconsistentNaming

namespace Kehlet.Functional;

public readonly struct Option<TValue>(TValue value) : IEquatable<Option<TValue>>
    where TValue : notnull
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

    public Option<TResult> Select<TResult>(Func<TValue, TResult> selector)
        where TResult : notnull =>
        IsSome
            ? some(selector(value))
            : none;

    public Option<TResult> Select<TResult>(Func<TValue, Option<TResult>> selector)
        where TResult : notnull =>
        IsSome
            ? selector(value)
            : none;

    public Option<TResult> SelectMany<TValue2, TResult>(Func<TValue, Option<TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
        where TResult : notnull 
        where TValue2 : notnull
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
        where TResult : notnull 
        where TValue2 : notnull
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
            ? $"Some({value})"
            : "None";

    public static implicit operator Option<TValue>(NoneOption _) => default;
    public static implicit operator Option<TValue>(OptionUnion<TValue>.Some option) => new(option.Value);
    public static implicit operator Option<TValue>(OptionUnion<TValue>.None _) => default;

    public bool Equals(Option<TValue> other) =>
        this == other;

    public override bool Equals(object? obj) =>
        obj is Option<TValue> other && this == other;

    public override int GetHashCode() =>
        IsSome
            ? EqualityComparer<TValue>.Default.GetHashCode(value)
            : 0;

    public static Option<TValue> operator |(Option<TValue> lhs, Option<TValue> rhs) => lhs.IsSome ? lhs : rhs;
    public static bool operator true(Option<TValue> option) => option.IsSome;
    public static bool operator false(Option<TValue> option) => !option.IsSome;

    public static bool operator ==(Option<TValue> lhs, Option<TValue> rhs) =>
        (lhs.IsSome, rhs.IsSome) switch
        {
            (true, true) => EqualityComparer<TValue>.Default.Equals(lhs.value, rhs.value),
            (false, false) => true,
            _ => false
        };

    public static bool operator !=(Option<TValue> lhs, Option<TValue> rhs) => !(lhs == rhs);
}

public readonly record struct NoneOption;

public static partial class Prelude
{
    public static Option<TValue> some<TValue>(TValue value)
        where TValue : notnull =>
        new(value);

    public static Option<(TValue1, TValue2)> some<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        new((value1, value2));

    public static NoneOption none =>
        default;

    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Option<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsSome
        select value.value;
}
