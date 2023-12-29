using Kehlet.Functional.Extensions;

namespace Kehlet.Functional;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct AsyncEffect<TRuntime, TValue>(Func<TRuntime, Task<Result<TValue>>> effect)
    where TValue : notnull
{
    public AsyncEffect<TRuntime, TResult> Select<TResult>(Func<TValue, TResult> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select Task.FromResult(selector(value)));
    }

    public AsyncEffect<TRuntime, TResult> Select<TResult>(Func<TValue, Task<TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select selector(value));
    }

    public AsyncEffect<TRuntime, TResult> Select<TResult>(Func<TValue, AsyncEffect<TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select selector(value).Run());
    }

    public AsyncEffect<TRuntime, TResult> Select<TResult>(Func<TValue, AsyncEffect<TRuntime, TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              select selector(value).Run(runtime));
    }

    public AsyncEffect<TRuntime, TResult> SelectMany<TValue2, TResult>(Func<TValue, AsyncEffect<TRuntime, TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              from value2 in selector(value).Run(runtime)
                              select resultSelector(value, value2));
    }

    public AsyncEffect<TRuntime, TResult> SelectMany<TValue2, TResult>(Func<TValue, AsyncEffect<TRuntime, TValue2>> selector, Func<TValue, TValue2, AsyncEffect<TRuntime, TResult>> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(runtime => from value in self.Run(runtime)
                              from value2 in selector(value).Run(runtime)
                              select resultSelector(value, value2).Run(runtime));
    }

    public async Task<Result<TValue>> Run(TRuntime runtime)
    {
        try
        {
            return await effect(runtime);
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    public static implicit operator AsyncEffect<TRuntime, TValue>(Func<TRuntime, Task<Result<TValue>>> f) =>
        new(f);

    public static implicit operator AsyncEffect<TRuntime, TValue>(Func<TRuntime, Task<Effect<TRuntime, TValue>>> f) =>
        new(runtime => f(runtime).Select(x => x.Run(runtime)));
}
