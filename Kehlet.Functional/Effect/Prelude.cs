namespace Kehlet.Functional;

public static partial class Prelude
{
    public static Effect<TValue> effect<TValue>(Func<Result<TValue>> effect)
        where TValue : notnull =>
        new(effect);
    
    public static Effect<TRuntime, TValue> effect<TRuntime, TValue>(Func<TRuntime, Result<TValue>> effect)
        where TValue : notnull =>
        new(effect);
}
