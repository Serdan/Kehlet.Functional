namespace Kehlet.Functional;

public static partial class Prelude
{
    [Pure]
    public static Option<TValue> some<TValue>(TValue value)
        where TValue : notnull =>
        new(value);

    [Pure]
    public static Option<(TValue1, TValue2)> some<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        new((value1, value2));

    public static NoneOption none =>
        default;

    [Pure]
    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Option<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsSome
        select value.value;

    [Pure]
    public static Func<Option<TSource>, Option<TResult>> map<TSource, TResult>(Func<TSource, TResult> f)
        where TSource : notnull
        where TResult : notnull =>
        opt => opt.Select(f);

    [Pure]
    public static Func<Option<TSource>, Option<TResult>> bind<TSource, TResult>(Func<TSource, Option<TResult>> f)
        where TSource : notnull
        where TResult : notnull =>
        opt => opt.Select(f);

    [Pure]
    public static Option<TResult> apply<TValue, TResult>(Option<Func<TValue, TResult>> selectorOption, Option<TValue> valueOption)
        where TValue : notnull
        where TResult : notnull =>
        from selector in selectorOption
        from value in valueOption
        select selector(value);

    [Pure]
    public static Option<Func<T2, TResult>> apply<T1, T2, TResult>(Option<Func<T1, T2, TResult>> selectorOption, Option<T1> valueOption)
        where T1 : notnull
        where TResult : notnull =>
        from selector in selectorOption
        from value in valueOption
        select curry(selector)(value);

    [Pure]
    public static Option<Func<T2, T3, TResult>> apply<T1, T2, T3, TResult>(Option<Func<T1, T2, T3, TResult>> selectorOption, Option<T1> valueOption)
        where T1 : notnull
        where TResult : notnull =>
        from selector in selectorOption
        from value in valueOption
        select curry2(selector)(value);

    [Pure]
    public static Option<Func<T2, T3, T4, TResult>> apply<T1, T2, T3, T4, TResult>(Option<Func<T1, T2, T3, T4, TResult>> selectorOption, Option<T1> valueOption)
        where T1 : notnull
        where TResult : notnull =>
        from selector in selectorOption
        from value in valueOption
        select curry2(selector)(value);

    [Pure]
    public static OrElse<TValue> orElse<TValue>(Option<TValue> option)
        where TValue : notnull =>
        new(option);

    [Pure]
    public static OrElse<TValue> orElse<TValue>(TValue value)
        where TValue : notnull =>
        new(some(value));

    public static Option<Func<T1, T3>> compose<T1, T2, T3>(Option<Func<T1, T2>> lhs, Option<Func<T2, T3>> rhs) =>
        from l in lhs
        from r in rhs
        select some<Func<T1, T3>>(x => r(l(x)));
}
