using System.Runtime.CompilerServices;

namespace Kehlet.Functional.Extensions;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class TaskExtensions
{
    [Pure]
    public static async Task<TValue> Where<TValue>(this Task<TValue> self, Func<TValue, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string expr = "")
    {
        var value = await self.NoSync();
        if (predicate(value))
        {
            return value;
        }
        else
        {
            throw new TaskCanceledException(expr);
        }
    }

    [Pure]
    public static async Task<TValue> Where<TValue>(this Task<TValue> self, Func<TValue, Task<bool>> predicate, [CallerArgumentExpression(nameof(predicate))] string expr = "")
    {
        var value = await self.NoSync();
        if (await predicate(value).NoSync())
        {
            return value;
        }
        else
        {
            throw new TaskCanceledException(expr);
        }
    }

    [Pure]
    public static async Task<TResult> Select<TValue, TResult>(this Task<TValue> self, Func<TValue, TResult> selector) =>
        selector(await self.NoSync());

    [Pure]
    public static async Task<TResult> Select<TValue, TResult>(this Task<TValue> self, Func<TValue, Task<TResult>> selector) =>
        await selector(await self.NoSync()).NoSync();

    [Pure]
    public static async Task<TResult> SelectMany<TValue, TTask, TResult>(
        this Task<TValue> self,
        Func<TValue, Task<TTask>> selector,
        Func<TValue, TTask, TResult> resultSelector)
    {
        var value = await self.NoSync();
        var value2 = await selector(value).NoSync();
        return resultSelector(value, value2);
    }

    [Pure]
    public static async Task<TResult> SelectMany<TValue, TTask, TResult>(
        this Task<TValue> self,
        Func<TValue, Task<TTask>> selector,
        Func<TValue, TTask, Task<TResult>> resultSelector)
    {
        var value = await self.NoSync();
        var value2 = await selector(value).NoSync();
        return await resultSelector(value, value2).NoSync();
    }

    [Pure]
    public static async Task<Unit> ToUnit(this Task task)
    {
        await task.NoSync();
        return unit;
    }

    [Pure]
    internal static ConfiguredTaskAwaitable<TValue> NoSync<TValue>(this Task<TValue> self) =>
        self.ConfigureAwait(false);

    [Pure]
    internal static ConfiguredTaskAwaitable NoSync(this Task self) =>
        self.ConfigureAwait(false);

    [Pure]
    public static async Task<Result<TResult>> Select<TValue, TResult>(this Task<Result<TValue>> self, Func<TValue, Task<TResult>> selector)
        where TValue : notnull
        where TResult : notnull
    {
        var result = await self.NoSync();
        if (result.IsError)
        {
            return error(result.error);
        }

        return ok(await selector(result.value));
    }

    [Pure]
    public static async Task<Result<TResult>> Select<TValue, TResult>(this Task<Result<TValue>> self, Func<TValue, Task<Result<TResult>>> selector)
        where TValue : notnull
        where TResult : notnull
    {
        var result = await self.NoSync();
        if (result.IsError)
        {
            return error(result.error);
        }

        return await selector(result.value).NoSync();
    }

    [Pure]
    public static async Task<Result<TResult>> SelectMany<TValue, TTask, TResult>(
        this Task<Result<TValue>> self,
        Func<TValue, Task<Result<TTask>>> selector,
        Func<TValue, TTask, TResult> resultSelector)
        where TValue : notnull
        where TTask : notnull
        where TResult : notnull
    {
        var result = await self.NoSync();
        if (result.IsError)
        {
            return error(result.error);
        }

        var result2 = await selector(result.value).NoSync();
        if (result2.IsError)
        {
            return error(result2.error);
        }

        return ok(resultSelector(result.value, result2.value));
    }

    [Pure]
    public static async Task<Result<TResult>> SelectMany<TValue, TTask, TResult>(
        this Task<Result<TValue>> self,
        Func<TValue, Task<Result<TTask>>> selector,
        Func<TValue, TTask, Task<Result<TResult>>> resultSelector)
        where TValue : notnull
        where TTask : notnull
        where TResult : notnull
    {
        var result = await self.NoSync();
        if (result.IsError)
        {
            return error(result.error);
        }

        var result2 = await selector(result.value).NoSync();
        if (result2.IsError)
        {
            return error(result2.error);
        }

        return await resultSelector(result.value, result2.value);
    }
}
