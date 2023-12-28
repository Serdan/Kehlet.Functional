using System.Collections;

namespace Kehlet.Functional;

public readonly partial struct Option<TValue>(TValue value) : IEquatable<Option<TValue>>, IEnumerable<TValue>
    where TValue : notnull
{
    internal readonly TValue value = value;

    public bool IsSome { get; } = true;

    public bool IsNone => !IsSome;

    [Pure]
    public TResult Match<TResult>(Func<TValue, TResult> some, Func<TResult> none) =>
        IsSome
            ? some(value)
            : none();

    [Pure]
    public Option<TValue> Where(Func<TValue, bool> predicate) =>
        IsSome && predicate(value)
            ? this
            : none;

    [Pure]
    public Option<TResult> Select<TResult>(Func<TValue, TResult> selector)
        where TResult : notnull =>
        IsSome
            ? some(selector(value))
            : none;

    [Pure]
    public Option<TResult> Select<TResult>(Func<TValue, Option<TResult>> selector)
        where TResult : notnull =>
        IsSome
            ? selector(value)
            : none;

    [Pure]
    [ComputationBuilderTargetType(typeof(OrElse<>))]
    public Option<TValue> Select(Func<TValue, OrElse<TValue>> orElseSelector) =>
        IsSome
            ? this
            : orElseSelector(default!).Option;

    [Pure]
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

    [Pure]
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

    [Pure]
    public Result<TValue> ToResult(string error) =>
        IsSome
            ? ok(value)
            : Prelude.error(error);

    [Pure]
    public override string ToString() =>
        IsSome
            ? $"Some({value})"
            : "None";

    [Pure]
    public bool Equals(Option<TValue> other) =>
        this == other;

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Option<TValue> other && this == other;

    [Pure]
    public override int GetHashCode() =>
        IsSome
            ? EqualityComparer<TValue>.Default.GetHashCode(value)
            : 0;

    [Pure]
    public Enumerator<TValue> GetEnumerator() =>
        new(this);

    [Pure]
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
        EnumeratorObject<TValue>.Create(this);

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        EnumeratorObject<TValue>.Create(this);

    public static implicit operator Option<TValue>(NoneOption _) => default;

    public static implicit operator Option<TValue>(OptionUnion<TValue>.Some option) => new(option.Value);

    public static implicit operator Option<TValue>(OptionUnion<TValue>.None _) => default;

    public static Option<TValue> operator |(Option<TValue> lhs, Option<TValue> rhs)
    {
        return lhs.IsSome
            ? lhs
            : rhs;
    }

    public static Option<IReadOnlyList<TValue>> operator &(Option<TValue> lhs, Option<TValue> rhs)
    {
        return lhs.IsSome && rhs.IsSome
            ? some((IReadOnlyList<TValue>) [lhs.value, rhs.value])
            : none;
    }

    public static Option<IReadOnlyList<TValue>> operator &(Option<IReadOnlyList<TValue>> lhs, Option<TValue> rhs)
    {
        return lhs.IsSome && rhs.IsSome
            ? some((IReadOnlyList<TValue>) [.. lhs.value, rhs.value])
            : none;
    }

    public static bool operator true(Option<TValue> option) => option.IsSome;

    public static bool operator false(Option<TValue> option) => !option.IsSome;

    public static bool operator ==(Option<TValue> lhs, Option<TValue> rhs)
    {
        return (lhs.IsSome, rhs.IsSome) switch
        {
            (true, true) => EqualityComparer<TValue>.Default.Equals(lhs.value, rhs.value),
            (false, false) => true,
            _ => false
        };
    }

    public static bool operator !=(Option<TValue> lhs, Option<TValue> rhs) => !(lhs == rhs);
}
