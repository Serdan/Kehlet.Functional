// ReSharper disable InconsistentNaming

namespace Kehlet.Functional;

public static partial class Prelude
{
    public static Unit unit { get; } = new();

    public static ResultUnion<TValue> union<TValue>(Result<TValue> result)
        where TValue : notnull =>
        result.IsOk
            ? ResultUnion<TValue>.Cons.NewOk(result.value)
            : ResultUnion<TValue>.Cons.NewError(result.error);

    public static OptionUnion<TValue> union<TValue>(Option<TValue> result)
        where TValue : notnull =>
        result.IsSome
            ? OptionUnion<TValue>.Cons.NewSome(result.value)
            : OptionUnion<TValue>.Cons.NewNone;

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

    public static TValue1 fst<TValue1, TValue2>((TValue1 value1, TValue2 _) tuple) => tuple.value1;

    public static TValue2 snd<TValue1, TValue2>((TValue1 _, TValue2 value2) tuple) => tuple.value2;

    public static Func<T1, T3> compose<T1, T2, T3>(Func<T1, T2> lhs, Func<T2, T3> rhs) => x => rhs(lhs(x));

    public static Func<T1, T4> compose<T1, T2, T3, T4>(Func<T1, T2> first, Func<T2, T3> second, Func<T3, T4> third) => x => third(second(first(x)));

    public static Fold<TAccumulate> fold<TAccumulate>(TAccumulate initialValue) =>
        new(initialValue);
}
