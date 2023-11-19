// ReSharper disable InconsistentNaming

namespace Kehlet.Functional;

public static partial class Prelude
{
    public static Unit unit { get; } = new();

    public static ResultUnion<TValue> union<TValue>(Result<TValue> result) =>
        result.IsOk
            ? ResultUnion<TValue>.Cons.NewOk(result.value)
            : ResultUnion<TValue>.Cons.NewError(result.error);

    public static OptionUnion<TValue> union<TValue>(Option<TValue> result) =>
        result.IsSome
            ? OptionUnion<TValue>.Cons.NewSome(result.value)
            : OptionUnion<TValue>.Cons.NewNone;

    public static Result<TValue> @try<TValue>(Action f, TValue success)
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
}
