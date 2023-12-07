// ReSharper disable InconsistentNaming

using System.Diagnostics.Contracts;

namespace Kehlet.Functional;

public static partial class Prelude
{
    public static Unit unit { get; } = new();

    [Pure]
    public static ResultUnion<TValue> union<TValue>(Result<TValue> result)
        where TValue : notnull =>
        result.IsOk
            ? ResultUnion<TValue>.Cons.NewOk(result.value)
            : ResultUnion<TValue>.Cons.NewError(result.error);

    [Pure]
    public static OptionUnion<TValue> union<TValue>(Option<TValue> result)
        where TValue : notnull =>
        result.IsSome
            ? OptionUnion<TValue>.Cons.NewSome(result.value)
            : OptionUnion<TValue>.Cons.NewNone;

    [Pure]
    public static Result<TValue> @try<TValue>(Action f, TValue success)
        where TValue : notnull
    {
        try
        {
            f();
            return ok(success);
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    [Pure]
    public static Result<TValue> @try<TValue>(Func<TValue> f)
        where TValue : notnull
    {
        try
        {
            return ok(f());
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    [Pure]
    public static Result<TValue> @try<TValue>(Func<Result<TValue>> f)
        where TValue : notnull
    {
        try
        {
            return f();
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    [Pure]
    public static TValue1 fst<TValue1, TValue2>((TValue1 value1, TValue2 _) tuple) =>
        tuple.value1;

    [Pure]
    public static TValue2 snd<TValue1, TValue2>((TValue1 _, TValue2 value2) tuple) =>
        tuple.value2;

    [Pure]
    public static Func<T1, T3> compose<T1, T2, T3>(
        Func<T1, T2> f1,
        Func<T2, T3> f2) =>
        x => f2(f1(x));

    [Pure]
    public static Func<T1, T4> compose<T1, T2, T3, T4>(
        Func<T1, T2> f1,
        Func<T2, T3> f2,
        Func<T3, T4> f3) =>
        x => f3(f2(f1(x)));

    [Pure]
    public static Func<T1, T5> compose<T1, T2, T3, T4, T5>(
        Func<T1, T2> f1,
        Func<T2, T3> f2,
        Func<T3, T4> f3,
        Func<T4, T5> f4) =>
        x => f4(f3(f2(f1(x))));

    [Pure]
    public static Fold<TAccumulate> fold<TAccumulate>(TAccumulate initialValue) =>
        new(initialValue);
}
