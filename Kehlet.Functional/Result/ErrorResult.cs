namespace Kehlet.Functional;

public readonly record struct ErrorResult(Exception Exception)
{
    [Pure]
    public Result<TValue> ToResult<TValue>()
        where TValue : notnull => this;
}
