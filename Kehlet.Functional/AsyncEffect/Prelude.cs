namespace Kehlet.Functional;

public static partial class Prelude
{
    public static AsyncEffect<TValue> asyncEffect<TValue>(Func<Task<Result<TValue>>> f)
        where TValue : notnull =>
        new(f);

    public static AsyncEffect<TRuntime, TValue> asyncEffect<TRuntime, TValue>(Func<TRuntime, Task<Result<TValue>>> f)
        where TValue : notnull =>
        new(f);

    public static AsyncEffect<TRuntime, TValue> asyncEffect<TRuntime, TValue>(Func<TRuntime, AsyncEffect<TValue>> e)
        where TValue : notnull =>
        new(runtime => e(runtime).Run());

    public static AsyncEffect<TRuntime, TValue> asyncEffect<TRuntime, TValue>(Func<TRuntime, AsyncEffect<TRuntime, TValue>> e)
        where TValue : notnull =>
        new(runtime => e(runtime).Run(runtime));

    public static AsyncEffect<TValue> okAsyncEffect<TValue>(TValue value)
        where TValue : notnull =>
        new(() => Task.FromResult(ok(value)));

    public static AsyncEffect<TRuntime, TValue> okAsyncEffect<TRuntime, TValue>(TValue value)
        where TValue : notnull =>
        new(_ => Task.FromResult(ok(value)));

    public static AsyncEffect<TValue> errorAsyncEffect<TValue>(string message)
        where TValue : notnull =>
        new(() => Task.FromResult(error(message).ToResult<TValue>()));

    public static AsyncEffect<TRuntime, TValue> errorAsyncEffect<TRuntime, TValue>(string message)
        where TValue : notnull =>
        new(_ => Task.FromResult(error(message).ToResult<TValue>()));

    public static AsyncEffect<IEnumerable<TValue>> filter<TValue>(IEnumerable<AsyncEffect<TValue>> enumerable)
        where TValue : notnull
    {
        return new(async () => ok((IEnumerable<TValue>) await Core().ToListAsync()));

        async IAsyncEnumerable<TValue> Core()
        {
            foreach (var item in enumerable)
            {
                var result = await item.Run();
                if (result.IsOk)
                {
                    yield return result.value;
                }
            }
        }
    }

    public static AsyncEffect<TRuntime, IEnumerable<TValue>> filter<TRuntime, TValue>(IEnumerable<AsyncEffect<TRuntime, TValue>> enumerable)
        where TValue : notnull
    {
        return new(async runtime => ok((IEnumerable<TValue>) await Core(runtime).ToListAsync()));

        async IAsyncEnumerable<TValue> Core(TRuntime runtime)
        {
            foreach (var item in enumerable)
            {
                var result = await item.Run(runtime);
                if (result.IsOk)
                {
                    yield return result.value;
                }
            }
        }
    }

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
