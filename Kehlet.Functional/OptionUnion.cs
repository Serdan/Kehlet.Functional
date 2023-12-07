using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Kehlet.Functional;

[Union]
public partial record OptionUnion<TValue>
{
    partial record Some(TValue Value);

    partial record None;

    [Pure]
    public OptionUnion<TValue> Where(Func<TValue, bool> predicate) =>
        this switch
        {
            Some(var value) when predicate(value) => this,
            _ => Cons.NewNone
        };

    [Pure]
    public OptionUnion<TResult> Select<TResult>(Func<TValue, TResult> selector) =>
        this switch
        {
            Some(var value) => OptionUnion<TResult>.Cons.NewSome(selector(value)),
            None => OptionUnion<TResult>.Cons.NewNone,
            _ => throw new UnreachableException()
        };

    [Pure]
    public OptionUnion<TResult> Select<TResult>(Func<TValue, OptionUnion<TResult>> selector) =>
        this switch
        {
            Some(var value) => selector(value),
            None => OptionUnion<TResult>.Cons.NewNone,
            _ => throw new UnreachableException()
        };

    [Pure]
    public OptionUnion<TResult> SelectMany<TValue2, TResult>(Func<TValue, OptionUnion<TValue2>> bind, Func<TValue, TValue2, TResult> map) =>
        this switch
        {
            Some(var value) => from value2 in bind(value) select map(value, value2),
            None => OptionUnion<TResult>.Cons.NewNone,
            _ => throw new UnreachableException()
        };

    [Pure]
    public OptionUnion<TResult> SelectMany<TValue2, TResult>(Func<TValue, OptionUnion<TValue2>> bind, Func<TValue, TValue2, OptionUnion<TResult>> map) =>
        this switch
        {
            Some(var value) => from value2 in bind(value) select map(value, value2),
            None => OptionUnion<TResult>.Cons.NewNone,
            _ => throw new UnreachableException()
        };
}
