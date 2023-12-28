namespace Kehlet.Functional;

public static partial class Prelude
{
    [Pure]
    public static Result<Unit> ok() => Result<Unit>.OkResult(unit);

    [Pure]
    public static Result<TValue> ok<TValue>(TValue value)
        where TValue : notnull =>
        Result<TValue>.OkResult(value);

    [Pure]
    public static Result<(TValue1, TValue2)> ok<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        Result<(TValue1, TValue2)>.OkResult((value1, value2));

    [Pure]
    public static ErrorResult error(Exception exception) =>
        new(exception);

    [Pure]
    public static ErrorResult error(string message) =>
        new(new(message));

    [Pure]
    public static Result<TValue> error<TValue>(string message)
        where TValue : notnull =>
        Result<TValue>.ErrorResult(new(message));

    [Pure]
    public static Result<TValue> error<TValue>(Exception exception)
        where TValue : notnull =>
        Result<TValue>.ErrorResult(exception);

    [Pure]
    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Result<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsOk
        select value.value;

    [Pure]
    public static IEnumerable<TValue> Filter<TValue>(this IEnumerable<Result<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsOk
        select value.value;
}
