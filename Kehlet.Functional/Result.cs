// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;

namespace Kehlet.Functional;

/// <summary>
/// Represents the outcome of a computation that can either succeed with a value or fail with an exception.
/// </summary>
/// <typeparam name="TValue">The type of the value returned in case of success.</typeparam>
public readonly struct Result<TValue> : IEquatable<Result<TValue>>
    where TValue : notnull
{
    internal readonly TValue value;
    internal readonly Exception error;

    /// <summary>
    /// Gets a value indicating whether the result is successful.
    /// </summary>
    public bool IsOk { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents an error.
    /// </summary>
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

    /// <summary>
    /// Applies one of the two provided functions based on the result state.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the provided functions.</typeparam>
    /// <param name="ok">A function to apply when the result is successful.</param>
    /// <param name="error">A function to apply when the result is an error.</param>
    /// <returns>The result of applying the selected function.</returns>
    public TResult Match<TResult>(Func<TValue, TResult> ok, Func<Exception, TResult> error) =>
        IsOk
            ? ok(value)
            : error(this.error);

    public Result<TValue> Where(Func<TValue, bool> f, [CallerArgumentExpression(nameof(f))] string expr = "") =>
        IsOk && !f(value)
            ? error(expr)
            : this;

    public Result<TValue> Where(Func<TValue, (bool, string)> f)
    {
        if (IsOk is false)
        {
            return this;
        }

        var (pred, err) = f(value);
        if (pred)
        {
            return this;
        }

        return error(err);
    }

    public Result<TValue> Where(Func<TValue, (bool, Exception)> f)
    {
        if (IsOk is false)
        {
            return this;
        }

        var (pred, err) = f(value);
        if (pred)
        {
            return this;
        }

        return error(err);
    }

    /// <summary>
    /// Transforms the value of the result using a specified function if the result is successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the result after applying the transformation function.</typeparam>
    /// <param name="f">A transformation function to apply to the value.</param>
    /// <returns>A new result object with the transformed value if the original result is successful, otherwise an error result.</returns>
    public Result<TResult> Select<TResult>(Func<TValue, TResult> f)
        where TResult : notnull =>
        IsOk
            ? ok(f(value))
            : error(error);

    /// <summary>
    /// Transforms the value of the result using a specified function that returns a result object if the original result is successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the transformation function.</typeparam>
    /// <param name="f">A transformation function to apply to the value that returns a result object.</param>
    /// <returns>A new result object from the transformation function if the original result is successful, otherwise an error result.</returns>
    public Result<TResult> Select<TResult>(Func<TValue, Result<TResult>> f)
        where TResult : notnull =>
        IsOk
            ? f(value)
            : error(error);

    /// <summary>
    /// Asynchronously transforms the value of the result using a specified function if the result is successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the result after applying the transformation function.</typeparam>
    /// <param name="f">An asynchronous transformation function to apply to the value.</param>
    /// <returns>A task representing the asynchronous operation, containing a new result object with the transformed value if the original result is successful, otherwise an error result.</returns>
    public async Task<Result<TResult>> Select<TResult>(Func<TValue, Task<TResult>> f)
        where TResult : notnull =>
        IsOk
            ? ok(await f(value))
            : error(error);

    /// <summary>
    /// Projects the value of the result to a new form using a specified selector function, then transforms the result of that function using another result selector function.
    /// </summary>
    /// <typeparam name="TValue2">The type of the intermediate result returned by the selector function.</typeparam>
    /// <typeparam name="TResult">The type of the final result returned by the result selector function.</typeparam>
    /// <param name="selector">A function to transform the original value to an intermediate result.</param>
    /// <param name="resultSelector">A function to transform the intermediate result to the final result.</param>
    /// <returns>A new result object representing the transformed value.</returns>
    public Result<TResult> SelectMany<TValue2, TResult>(Func<TValue, Result<TValue2>> selector, Func<TValue, TValue2, TResult> resultSelector)
        where TResult : notnull
        where TValue2 : notnull
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

    /// <summary>
    /// Projects the value of the result to a new form using a specified selector function, then transforms the result of that function using another function that returns a new result object.
    /// </summary>
    /// <typeparam name="TValue2">The type of the intermediate result returned by the selector function.</typeparam>
    /// <typeparam name="TResult">The type of the final result returned by the result selector function.</typeparam>
    /// <param name="selector">A function to transform the original value to an intermediate result.</param>
    /// <param name="resultSelector">A function to transform the intermediate result to a final result object.</param>
    /// <returns>A new result object representing the final transformed value.</returns>
    public Result<TResult> SelectMany<TValue2, TResult>(Func<TValue, Result<TValue2>> selector, Func<TValue, TValue2, Result<TResult>> resultSelector)
        where TResult : notnull
        where TValue2 : notnull
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

    /// <summary>
    /// Returns the value if the result is successful, or applies a specified function to the error if it's not.
    /// </summary>
    /// <param name="f">A function to apply to the error when the result is not successful.</param>
    /// <returns>The original value if the result is successful; otherwise, the result of the function applied to the error.</returns>
    public TValue IfError(Func<Exception, TValue> f) =>
        IsOk
            ? value
            : f(error);

    /// <summary>
    /// Returns the original result if it is successful, or applies a specified function to the error if it's not, returning a new result object.
    /// </summary>
    /// <param name="f">A function to apply to the error when the result is not successful, returning a new result object.</param>
    /// <returns>The original result if it is successful; otherwise, the new result object.</returns>
    public Result<TValue> SelectError(Func<Exception, Result<TValue>> f) =>
        IsOk
            ? this
            : f(error);

    /// <summary>
    /// Transforms the error of the result using a specified function if the result is not successful.
    /// </summary>
    /// <param name="f">A transformation function to apply to the error.</param>
    /// <returns>A new result object with the transformed error if the original result is not successful, otherwise the original result.</returns>
    public Result<TValue> SelectError(Func<Exception, Exception> f) =>
        IsOk
            ? this
            : error(f(error));

    /// <summary>
    /// Returns a string representation of the result, showing a successful value or the error message.
    /// </summary>
    /// <returns>A string representation of the result.</returns>
    public override string ToString() =>
        IsOk
            ? $"Ok({value})"
            : $"Error({error.Message})";

    /// <summary>
    /// Creates a successful result object with the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be included in the result.</typeparam>
    /// <param name="value">The value to include in the result.</param>
    /// <returns>A successful result object containing the provided value.</returns>
    public static Result<TValue> OkResult(TValue value) => new(value);

    /// <summary>
    /// Creates an error result object with the provided exception.
    /// </summary>
    /// <param name="exception">The exception to include in the error result.</param>
    /// <returns>An error result object containing the provided exception.</returns>
    public static Result<TValue> ErrorResult(Exception exception) => new(exception);

    /// <summary>
    /// Implicitly converts an ErrorResult to a Result object, encapsulating the provided exception.
    /// </summary>
    /// <param name="errorResult">The ErrorResult to convert.</param>
    /// <returns>A Result object representing the error.</returns>
    public static implicit operator Result<TValue>(ErrorResult errorResult) => ErrorResult(errorResult.Exception);

    /// <summary>
    /// Implicitly converts a ResultUnion.Ok to a Result object, encapsulating the successful value.
    /// </summary>
    /// <param name="result">The ResultUnion.Ok to convert.</param>
    /// <returns>A Result object representing the successful value.</returns>
    public static implicit operator Result<TValue>(ResultUnion<TValue>.Ok result) => OkResult(result.Value);

    /// <summary>
    /// Implicitly converts a ResultUnion.Error to a Result object, encapsulating the provided exception.
    /// </summary>
    /// <param name="result">The ResultUnion.Error to convert.</param>
    /// <returns>A Result object representing the error.</returns>
    public static implicit operator Result<TValue>(ResultUnion<TValue>.Error result) => ErrorResult(result.Exception);

    public bool Equals(Result<TValue> other) =>
        this == other;

    public override bool Equals(object? obj) =>
        obj is Result<TValue> other && Equals(other);

    public override int GetHashCode() =>
        IsOk
            ? EqualityComparer<TValue>.Default.GetHashCode(value)
            : EqualityComparer<Exception>.Default.GetHashCode(error);

    /// <summary>
    /// Defines a custom operator | to select the first successful result between two results.
    /// </summary>
    /// <param name="lhs">The first result to evaluate.</param>
    /// <param name="rhs">The second result to evaluate.</param>
    /// <returns>The first successful result if either is successful, otherwise the second result.</returns>
    public static Result<TValue> operator |(Result<TValue> lhs, Result<TValue> rhs) =>
        lhs.IsOk
            ? lhs
            : rhs;

    public static bool operator true(Result<TValue> result) => result.IsOk;

    public static bool operator false(Result<TValue> result) => !result.IsOk;

    public static bool operator ==(Result<TValue> lhs, Result<TValue> rhs) =>
        (lhs.IsOk, rhs.IsOk) switch
        {
            (true, true) => EqualityComparer<TValue>.Default.Equals(rhs.value, lhs.value),
            (false, false) => EqualityComparer<Exception>.Default.Equals(lhs.error, rhs.error),
            _ => false
        };

    public static bool operator !=(Result<TValue> lhs, Result<TValue> rhs) => !(lhs == rhs);
}

public readonly record struct ErrorResult(Exception Exception)
{
    public Result<TValue> ToResult<TValue>()
        where TValue : notnull => this;
}

public static partial class Prelude
{
    public static Result<Unit> ok() => Result<Unit>.OkResult(unit);
    
    public static Result<TValue> ok<TValue>(TValue value)
        where TValue : notnull =>
        Result<TValue>.OkResult(value);

    public static Result<(TValue1, TValue2)> ok<TValue1, TValue2>(TValue1 value1, TValue2 value2) =>
        Result<(TValue1, TValue2)>.OkResult((value1, value2));

    public static ErrorResult error(Exception exception) =>
        new(exception);

    public static ErrorResult error(string message) =>
        new(new(message));

    public static Result<TValue> error<TValue>(string message)
        where TValue : notnull =>
        Result<TValue>.ErrorResult(new(message));

    public static Result<TValue> error<TValue>(Exception exception)
        where TValue : notnull =>
        Result<TValue>.ErrorResult(exception);

    public static IEnumerable<TValue> filter<TValue>(IEnumerable<Result<TValue>> values)
        where TValue : notnull =>
        from value in values
        where value.IsOk
        select value.value;
}
