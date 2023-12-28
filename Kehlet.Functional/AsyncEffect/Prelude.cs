namespace Kehlet.Functional;

public static partial class Prelude
{
    public static AsyncEffect<TValue> effect<TValue>(Func<Task<Result<TValue>>> effect)
        where TValue : notnull =>
        new(effect);

    public static AsyncEffect<TRuntime, TValue> effect<TRuntime, TValue>(Func<TRuntime, Task<Result<TValue>>> effect)
        where TValue : notnull =>
        new(effect);

    public static AsyncEffect<TRuntime, (TValue1, TValue2)> combine<TRuntime, TValue1, TValue2>(
        AsyncEffect<TRuntime, TValue1> value1,
        AsyncEffect<TRuntime, TValue2> value2)
        where TValue1 : notnull
        where TValue2 : notnull =>
        from v1 in value1
        from v2 in value2
        select (v1, v2);

    public static AsyncEffect<TRuntime, (TValue1, TValue2, TValue3)> combine<TRuntime, TValue1, TValue2, TValue3>(
        AsyncEffect<TRuntime, TValue1> value1,
        AsyncEffect<TRuntime, TValue2> value2,
        AsyncEffect<TRuntime, TValue3> value3)
        where TValue1 : notnull
        where TValue2 : notnull
        where TValue3 : notnull =>
        from v1 in value1
        from v2 in value2
        from v3 in value3
        select (v1, v2, v3);
}
