using System.Diagnostics;

namespace Kehlet.Functional;

[Union]
public partial record OptionUnion<TValue>
{
    partial record Some(TValue Value);

    partial record None;

    public OptionUnion<TResult> Select<TResult>(Func<TValue, TResult> selector) =>
        this switch
        {
            Some(var value) => OptionUnion<TResult>.Cons.NewSome(selector(value)),
            None => OptionUnion<TResult>.Cons.NewNone,
            _ => throw new UnreachableException()
        };

    public OptionUnion<TResult> Select<TResult>(Func<TValue, OptionUnion<TResult>> selector) =>
        this switch
        {
            Some(var value) => selector(value),
            None => OptionUnion<TResult>.Cons.NewNone,
            _ => throw new UnreachableException()
        };

    public OptionUnion<TResult> SelectMany<TValue2, TResult>(Func<TValue, OptionUnion<TValue2>> bind, Func<TValue, TValue2, TResult> map)
    {
        if (this is not Some(var value))
        {
            return OptionUnion<TResult>.Cons.NewNone;
        }

        var option2 = bind(value);

        if (option2 is not OptionUnion<TValue2>.Some(var value2))
        {
            return OptionUnion<TResult>.Cons.NewNone;
        }

        return OptionUnion<TResult>.Cons.NewSome(map(value, value2));
    }

    public OptionUnion<TResult> SelectMany<TValue2, TResult>(Func<TValue, OptionUnion<TValue2>> bind, Func<TValue, TValue2, OptionUnion<TResult>> map)
    {
        if (this is not Some(var value))
        {
            return OptionUnion<TResult>.Cons.NewNone;
        }

        var option2 = bind(value);

        if (option2 is not OptionUnion<TValue2>.Some(var value2))
        {
            return OptionUnion<TResult>.Cons.NewNone;
        }

        return map(value, value2);
    }
}
