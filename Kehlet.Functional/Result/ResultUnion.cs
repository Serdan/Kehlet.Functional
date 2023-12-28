using System.Diagnostics;

namespace Kehlet.Functional;

[Union]
public partial record ResultUnion<TValue>
{
    partial record Ok(TValue Value);

    partial record Error(Exception Exception);

    [Pure]
    public ResultUnion<TResult> Select<TResult>(Func<TValue, TResult> selector) =>
        this switch
        {
            Ok(var value) => ResultUnion<TResult>.Cons.NewOk(selector(value)),
            Error(var error) => ResultUnion<TResult>.Cons.NewError(error),
            _ => throw new UnreachableException()
        };

    [Pure]
    public ResultUnion<TResult> Select<TResult>(Func<TValue, ResultUnion<TResult>> selector) =>
        this switch
        {
            Ok(var value) => selector(value),
            Error(var error) => ResultUnion<TResult>.Cons.NewError(error),
            _ => throw new UnreachableException()
        };

    [Pure]
    public ResultUnion<TResult> SelectMany<TValue2, TResult>(Func<TValue, ResultUnion<TValue2>> bind, Func<TValue, TValue2, TResult> map) =>
        this switch
        {
            Ok(var value) => from value2 in bind(value) select map(value, value2),
            Error(var error) => ResultUnion<TResult>.Cons.NewError(error),
            _ => throw new UnreachableException()
        };

    [Pure]
    public ResultUnion<TResult> SelectMany<TValue2, TResult>(Func<TValue, ResultUnion<TValue2>> bind, Func<TValue, TValue2, ResultUnion<TResult>> map) =>
        this switch
        {
            Ok(var value) => from value2 in bind(value) select map(value, value2),
            Error(var error) => ResultUnion<TResult>.Cons.NewError(error),
            _ => throw new UnreachableException()
        };
}
