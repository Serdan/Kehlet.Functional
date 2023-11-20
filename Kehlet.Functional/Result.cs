// ReSharper disable InconsistentNaming

namespace Kehlet.Functional;

public readonly struct Result<TValue>
{
    internal readonly TValue value;
    internal readonly Exception error;

    public bool IsOk { get; }
    public bool IsError => !IsOk;

    private Result(TValue value)
    {
        this.value = value;
        error = null!;
        IsOk = true;
    }

    private Result(Exception error)
    {
        value = default!;
        this.error = error;
        IsOk = false;
    }

    public TResult Match<TResult>(Func<TValue, TResult> ok, Func<Exception, TResult> error) =>
        IsOk
            ? ok(value)
            : error(this.error);

    public Result<TResult> Select<TResult>(Func<TValue, TResult> f) =>
        IsOk
            ? ok(f(value))
            : error(error);

    public Result<TResult> Select<TResult>(Func<TValue, Result<TResult>> f) =>
        IsOk
            ? f(value)
            : error(error);

    public async Task<Result<TResult>> Select<TResult>(Func<TValue, Task<TResult>> f) =>
        IsOk
            ? ok(await f(value))
            : error(error);

    public Result<TResult> SelectMany<TValue2, TResult>(Func<TValue, Result<TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
    {
        if (IsError)
        {
            return error(error);
        }

        var inner = selector(value);

        if (inner.IsError)
        {
            return error(inner.error);
        }

        return ok(resultSelector(value, inner.value));
    }

    public Result<TResult> SelectMany<TValue2, TResult>(Func<TValue, Result<TValue2>> selector, Func<TValue, TValue2, Result<TResult>> resultSelector)
    {
        if (IsError)
        {
            return error(error);
        }

        var inner = selector(value);

        if (inner.IsError)
        {
            return error(inner.error);
        }

        return resultSelector(value, inner.value);
    }

    public TValue IfError(Func<Exception, TValue> f) =>
        IsOk
            ? value
            : f(error);

    public Result<TValue> SelectError(Func<Exception, Exception> f) =>
        IsOk
            ? this
            : error(f(error));

    public override string ToString() =>
        IsOk
            ? $"ok {value}"
            : $"error {error.Message}";

    public static Result<TValue> OkResult(TValue value) => new(value);
    public static Result<TValue> ErrorResult(Exception exception) => new(exception);

    public static implicit operator Result<TValue>(ErrorResult errorResult) => ErrorResult(errorResult.Exception);
    public static implicit operator Result<TValue>(ResultUnion<TValue>.Ok result) => OkResult(result.Value);
    public static implicit operator Result<TValue>(ResultUnion<TValue>.Error result) => ErrorResult(result.Exception);

    public static Result<TValue> operator |(Result<TValue> a, Result<TValue> b) => a.IsOk ? a : b;
}

public readonly record struct ErrorResult(Exception Exception);

public static partial class Prelude
{
    public static Result<TValue> ok<TValue>(TValue value) =>
        Result<TValue>.OkResult(value);

    public static Result<(TValue1, TValue2)> ok<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        Result<(TValue1, TValue2)>.OkResult((value1, value2));

    public static ErrorResult error(Exception exception) =>
        new(exception);

    public static ErrorResult error(string message) =>
        new(new(message));

    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Result<TValue>> values) =>
        from value in values
        where value.IsOk
        select value.value;
}
