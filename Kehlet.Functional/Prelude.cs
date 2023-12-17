// ReSharper disable InconsistentNaming

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Kehlet.Functional;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
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

    [Pure]
    public static Func<T1, Func<T2, T3>> curry<T1, T2, T3>(Func<T1, T2, T3> f) =>
        t1 => t2 => f(t1, t2);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, T4>>> curry<T1, T2, T3, T4>(Func<T1, T2, T3, T4> f) =>
        t1 => t2 => t3 => f(t1, t2, t3);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, T5>>>> curry<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> f) =>
        t1 => t2 => t3 => t4 => f(t1, t2, t3, t4);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, T6>>>>> curry<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> f) =>
        t1 => t2 => t3 => t4 => t5 => f(t1, t2, t3, t4, t5);

    [Pure]
    public static Func<T1, Func<T2, T3, T4>> curry2<T1, T2, T3, T4>(Func<T1, T2, T3, T4> selector) =>
        t1 => (t2, t3) => selector(t1, t2, t3);

    [Pure]
    public static Func<T1, Func<T2, T3, T4, T5>> curry2<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> selector) =>
        t1 => (t2, t3, t4) => selector(t1, t2, t3, t4);

    [Pure]
    public static Func<T1, Func<T2, T3, T4, T5, T6>> curry2<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> selector) =>
        t1 => (t2, t3, t4, t5) => selector(t1, t2, t3, t4, t5);
}
