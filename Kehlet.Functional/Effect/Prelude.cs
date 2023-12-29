namespace Kehlet.Functional;

public static partial class Prelude
{
    public static Effect<TValue> effect<TValue>(Func<Result<TValue>> f)
        where TValue : notnull =>
        new(f);

    public static Effect<TRuntime, TValue> effect<TRuntime, TValue>(Func<TRuntime, Result<TValue>> f)
        where TValue : notnull =>
        new(f);

    public static Effect<TRuntime, TValue> effect<TRuntime, TValue>(Func<TRuntime, Effect<TValue>> f)
        where TValue : notnull =>
        new(runtime => f(runtime).Run());

    public static Effect<TRuntime, TValue> effect<TRuntime, TValue>(Func<TRuntime, Effect<TRuntime, TValue>> f)
        where TValue : notnull =>
        new(runtime => f(runtime).Run(runtime));

    public static Effect<TValue> okEffect<TValue>(TValue value)
        where TValue : notnull =>
        new(() => ok(value));

    public static Effect<TRuntime, TValue> okEffect<TRuntime, TValue>(TValue value)
        where TValue : notnull =>
        new(_ => ok(value));

    public static Effect<TValue> okEffect<TValue>(Func<TValue> f)
        where TValue : notnull =>
        new(() => ok(f()));

    public static Effect<TRuntime, TValue> okEffect<TRuntime, TValue>(Func<TValue> f)
        where TValue : notnull =>
        new(_ => ok(f()));

    public static Effect<TValue> errorEffect<TValue>(string message)
        where TValue : notnull =>
        new(() => error(message).ToResult<TValue>());

    public static Effect<TValue> errorEffect<TValue>(Exception exception)
        where TValue : notnull =>
        new(() => error(exception).ToResult<TValue>());

    public static Effect<TValue> errorEffect<TRuntime, TValue>(string message)
        where TValue : notnull =>
        new(() => error(message).ToResult<TValue>());

    public static Effect<TValue> errorEffect<TRuntime, TValue>(Exception exception)
        where TValue : notnull =>
        new(() => error(exception).ToResult<TValue>());

    public static Effect<IEnumerable<TValue>> filter<TValue>(IEnumerable<Effect<TValue>> enumerable)
        where TValue : notnull
    {
        return okEffect(Core);

        IEnumerable<TValue> Core()
        {
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in enumerable)
            {
                var result = item.Run();
                if (result.IsOk)
                {
                    yield return result.value;
                }
            }
        }
    }

    public static Effect<TRuntime, IEnumerable<TValue>> filter<TRuntime, TValue>(IEnumerable<Effect<TValue>> enumerable)
        where TValue : notnull
    {
        return okEffect<TRuntime, IEnumerable<TValue>>(Core);

        IEnumerable<TValue> Core()
        {
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in enumerable)
            {
                var result = item.Run();
                if (result.IsOk)
                {
                    yield return result.value;
                }
            }
        }
    }
}
