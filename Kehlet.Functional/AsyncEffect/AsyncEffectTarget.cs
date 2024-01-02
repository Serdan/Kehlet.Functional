namespace Kehlet.Functional;

[UsedImplicitly]
public readonly ref struct AsyncEffectTarget<TValue>
    where TValue : notnull
{
    public static AsyncEffect<TValue> operator <<(AsyncEffectTarget<TValue> _, Task<Result<TValue>> result) =>
        new(() => result);

    public static AsyncEffect<TValue> operator <<(AsyncEffectTarget<TValue> _, AsyncEffect<TValue> effect) =>
        new(effect.Run);

    public static AsyncEffect<TValue> operator <<(AsyncEffectTarget<TValue> _, Func<Task<Result<TValue>>> f) =>
        new(f);

    public static AsyncEffect<TValue> operator <<(AsyncEffectTarget<TValue> _, Func<AsyncEffect<TValue>> f) =>
        new(() => f().Run());
}

[UsedImplicitly]
public readonly ref struct AsyncEffectTarget<TRuntime, TValue>
    where TValue : notnull
{
    public static AsyncEffect<TRuntime, TValue> operator <<(AsyncEffectTarget<TRuntime, TValue> _, AsyncEffect<TRuntime, TValue> effect) =>
        new(effect.Run);

    public static AsyncEffect<TRuntime, TValue> operator <<(AsyncEffectTarget<TRuntime, TValue> _, Func<TRuntime, Task<Result<TValue>>> f) =>
        new(f);

    public static AsyncEffect<TRuntime, TValue> operator <<(AsyncEffectTarget<TRuntime, TValue> _, Func<TRuntime, AsyncEffect<TRuntime, TValue>> f) =>
        new(runtime => f(runtime).Run(runtime));
}
