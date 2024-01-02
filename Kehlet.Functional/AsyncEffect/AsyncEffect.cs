using Kehlet.Functional.Extensions;

namespace Kehlet.Functional;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct AsyncEffect<TValue>(Func<Task<Result<TValue>>> effect)
    where TValue : notnull
{
    public AsyncEffect<TResult> Match<TResult>(Func<TValue, TResult> okCase, Func<Exception, TResult> errorCase)
        where TResult : notnull
    {
        var self = this;
        return new(() => from result in self.Run()
                         select result.Match(okCase, errorCase).Apply(ok));
    }

    public AsyncEffect<TResult> Select<TResult>(Func<TValue, Task<TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         select selector(value));
    }

    public AsyncEffect<TResult> Select<TResult>(Func<TValue, AsyncEffect<TResult>> selector)
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         select selector(value).Run());
    }

    public AsyncEffect<TResult> SelectMany<TValue2, TResult>(Func<TValue, AsyncEffect<TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         from value2 in selector(value).Run()
                         select resultSelector(value, value2));
    }

    public AsyncEffect<TResult> SelectMany<TValue2, TResult>(Func<TValue, AsyncEffect<TValue2>> selector, Func<TValue, TValue2, AsyncEffect<TResult>> resultSelector)
        where TValue2 : notnull
        where TResult : notnull
    {
        var self = this;
        return new(() => from value in self.Run()
                         from value2 in selector(value).Run()
                         select resultSelector(value, value2).Run());
    }

    public async Task<Result<TValue>> Run()
    {
        try
        {
            return await effect();
        }
        catch (Exception e)
        {
            return error(e);
        }
    }

    public AsyncEffect<TRuntime, TValue> WithRuntime<TRuntime>()
    {
        var self = this;
        return asyncEffect<TRuntime, TValue>(_ => self.Run());
    }

    public static implicit operator AsyncEffect<TValue>(Func<Task<Result<TValue>>> f) =>
        new(f);

    public static implicit operator AsyncEffect<TValue>(Func<Task<Effect<TValue>>> f) =>
        new(() => f().Select(x => x.Run()));

    public static implicit operator AsyncEffect<TValue>(Effect<TValue> eff) =>
        eff.ToAsync();
}
