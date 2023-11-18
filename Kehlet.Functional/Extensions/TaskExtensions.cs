namespace Kehlet.Functional.Extensions;

public static class TaskExtensions
{
    public static async Task<TResult> Select<TValue, TResult>(this Task<TValue> self, Func<TValue, TResult> f) =>
        f(await self);

    public static async Task<TResult> Select<TValue, TResult>(this Task<TValue> self, Func<TValue, Task<TResult>> f) =>
        await f(await self);

    public static async Task<TResult> SelectMany<TValue, TTask, TResult>(
        this Task<TValue> self,
        Func<TValue, Task<TTask>> selector,
        Func<TValue, TTask, TResult> resultSelector)
    {
        var value = await self;
        var value2 = await selector(value);
        return resultSelector(value, value2);
    }
    
    public static async Task<TResult> SelectMany<TValue, TTask, TResult>(
        this Task<TValue> self,
        Func<TValue, Task<TTask>> selector,
        Func<TValue, TTask, Task<TResult>> resultSelector)
    {
        var value = await self;
        var value2 = await selector(value);
        return await resultSelector(value, value2);
    }

    public static async Task<Unit> ToUnit(this Task task)
    {
        await task;
        return unit;
    }
}
