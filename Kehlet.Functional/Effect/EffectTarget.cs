namespace Kehlet.Functional;

[UsedImplicitly]
public readonly ref struct EffectTarget<TValue>
    where TValue : notnull
{
    public static Effect<TValue> operator <<(EffectTarget<TValue> _, Result<TValue> result) =>
        new(() => result);

    public static Effect<TValue> operator <<(EffectTarget<TValue> _, Effect<TValue> effect) =>
        new(effect.Run);

    public static Effect<TValue> operator <<(EffectTarget<TValue> _, Func<Result<TValue>> f) =>
        new(f);

    public static Effect<TValue> operator <<(EffectTarget<TValue> _, Func<Effect<TValue>> f) =>
        new(() => f().Run());
}

[UsedImplicitly]
public readonly ref struct EffectTarget<TRuntime, TValue>
    where TValue : notnull
{
    public static Effect<TRuntime, TValue> operator <<(EffectTarget<TRuntime, TValue> _, Effect<TRuntime, TValue> effect) =>
        new(effect.Run);

    public static Effect<TRuntime, TValue> operator <<(EffectTarget<TRuntime, TValue> _, Func<TRuntime, Result<TValue>> f) =>
        new(f);

    public static Effect<TRuntime, TValue> operator <<(EffectTarget<TRuntime, TValue> _, Func<TRuntime, Effect<TRuntime, TValue>> f) =>
        new(runtime => f(runtime).Run(runtime));
}
