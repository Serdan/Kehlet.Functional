using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Kehlet.Functional.Extensions;

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
}
