namespace Kehlet.Functional;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct Effect<TValue>(Func<Result<TValue>> effect)
    where TValue : notnull
{
    public Effect<TResult> Match<TResult>(Func<TValue, TResult> okCase, Func<Exception, TResult> errorCase)
        where TResult : notnull
    {
        var self = this;
        return new(() => ok(self.Run().Match(okCase, errorCase)));
    }

    public Effect<TResult> Select<TResult>(Func<TValue, TResult> selector)
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         select selector(value));
    }

    public Effect<TResult> Select<TResult>(Func<TValue, Effect<TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         select selector(value).Run());
    }

    [ComputationBuilderTargetType(typeof(Run))]
    public Result<TValue> Select(Func<TValue, Run> run) =>
        Run();

    public Effect<TResult> SelectMany<TValue2, TResult>(Func<TValue, Effect<TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         from value2 in selector(value).Run()
                         select resultSelector(value, value2));
    }

    public Effect<TResult> SelectMany<TValue2, TResult>(Func<TValue, Effect<TValue2>> selector, Func<TValue, TValue2, Effect<TResult>> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         from value2 in selector(value).Run()
                         select resultSelector(value, value2).Run());
    }

    public Result<TValue> Run()
    {
        try
        {
            return effect();
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    public AsyncEffect<TValue> ToAsync()
    {
        var self = this;
        return asyncEffect(() => Task.FromResult(self.Run()));
    }

    public Effect<TRuntime, TValue> WithRuntime<TRuntime>()
    {
        var self = this;
        return effect<TRuntime, TValue>(_ => self.Run());
    }

    public static implicit operator Effect<TValue>(Func<Result<TValue>> f) =>
        new(f);

    public static implicit operator Effect<TValue>(Func<Effect<TValue>> f) =>
        new(() => f().Run());
}
