namespace Kehlet.Functional.LinqPlus;

public static partial class Prelude
{
    [ComputationBuilderType]
    public readonly record struct First(bool Predicate);

    [ComputationBuilderType]
    public readonly record struct Last(bool Predicate);

    [ComputationBuilderType]
    public readonly record struct Take(int Count);

    [ComputationBuilderType]
    public readonly record struct TakeRange(Range Range);

    [ComputationBuilderType]
    public readonly record struct TakeWhile(bool Predicate);

    [ComputationBuilderType]
    public readonly record struct Reverse;

    [ComputationBuilderType]
    public readonly record struct Distinct;

    [ComputationBuilderType]
    public readonly record struct DistinctBy<TKey>(TKey Key);

    [ComputationBuilderType]
    public readonly record struct AwaitTask<TSource>(Task<TSource> Task);

    [ComputationBuilderType]
    public readonly record struct YieldAll<TSource>(IEnumerable<TSource> Source);

    [ComputationBuilderType]
    public readonly record struct AsType<TSource, TResult>(Func<IEnumerable<TSource>, TResult> Constructor);
}
