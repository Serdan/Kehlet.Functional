namespace Kehlet.Functional;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct Effect<TRuntime, TValue>(Func<TRuntime, Result<TValue>> effect)
    where TValue : notnull
{
    public Effect<TRuntime, TResult> Select<TResult>(Func<TValue, TResult> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select selector(value));
    }

    public Effect<TRuntime, TResult> Select<TResult>(Func<TValue, Effect<TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select selector(value).Run());
    }

    public Effect<TRuntime, TResult> Select<TResult>(Func<TValue, Effect<TRuntime, TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select selector(value).Run(runtime));
    }

    [ComputationBuilderTargetType(typeof(Run<>))]
    public Result<TValue> Select(Func<TValue, Run<TRuntime>> run) =>
        Run(run(default!).runtime);

    public Effect<TRuntime, TResult> SelectMany<TValue2, TResult>(Func<TValue, Effect<TRuntime, TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              from value2 in selector(value).Run(runtime)
                              select resultSelector(value, value2));
    }

    public Effect<TRuntime, TResult> SelectMany<TValue2, TResult>(Func<TValue, Effect<TRuntime, TValue2>> selector, Func<TValue, TValue2, Effect<TRuntime, TResult>> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              from value2 in selector(value).Run(runtime)
                              select resultSelector(value, value2).Run(runtime));
    }

    public Result<TValue> Run(TRuntime runtime)
    {
        try
        {
            return effect(runtime);
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    public AsyncEffect<TRuntime, TValue> ToAsync()
    {
        var self = this;
        return asyncEffect<TRuntime, TValue>(runtime => Task.FromResult(self.Run(runtime)));
    }

    public static implicit operator Effect<TRuntime, TValue>(Func<TRuntime, Result<TValue>> f) =>
        new(f);

    public static implicit operator Effect<TRuntime, TValue>(Func<TRuntime, Effect<TRuntime, TValue>> f) =>
        new(runtime => f(runtime).Run(runtime));
}
