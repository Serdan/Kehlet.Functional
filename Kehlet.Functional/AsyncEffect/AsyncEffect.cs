﻿using Kehlet.Functional.Extensions;

namespace Kehlet.Functional;

public readonly struct AsyncEffect<TValue>(Func<Task<Result<TValue>>> effect)
    where TValue : notnull
{
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
        return effect<TRuntime, TValue>(_ => self.Run());
    }
}