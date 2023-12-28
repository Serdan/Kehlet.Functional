using System.Runtime.CompilerServices;

namespace Kehlet.Functional.LinqPlus;

public static partial class Prelude
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(First))]
    public static Option<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, First> first)
        where TSource : notnull =>
        source.Select(item => first(item).Predicate
                          ? some(item)
                          : none)
              .FirstOrDefault(item => item.IsSome, none);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(First))]
    public static Option<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, int, First> first)
        where TSource : notnull =>
        source.Select((item, index) => first(item, index).Predicate
                          ? some(item)
                          : none)
              .FirstOrDefault(item => item.IsSome, none);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(Last))]
    public static Option<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, Last> predicate)
        where TSource : notnull =>
        source.Select(item => predicate(item).Predicate
                          ? some(item)
                          : none)
              .LastOrDefault(item => item.IsSome, none);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(Last))]
    public static Option<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, int, Last> predicate)
        where TSource : notnull =>
        source.Select((item, index) => predicate(item, index).Predicate
                          ? some(item)
                          : none)
              .LastOrDefault(item => item.IsSome, none);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(Take))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, Take> take) =>
        source.Take(take(default!).Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(TakeRange))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, TakeRange> take) =>
        source.Take(take(default!).Range);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(TakeWhile))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, TakeWhile> takeWhile) =>
        source.TakeWhile(item => takeWhile(item).Predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(TakeWhile))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, int, TakeWhile> takeWhile) =>
        source.TakeWhile((item, index) => takeWhile(item, index).Predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(Reverse))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, Reverse> reverse) =>
        source.Reverse();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(Distinct))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, Distinct> distinct) =>
        source.Distinct();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(DistinctBy<>))]
    public static IEnumerable<TSource> Select<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, DistinctBy<TKey>> distinct) =>
        source.DistinctBy(item => distinct(item).Key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(YieldAll<>))]
    public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> source, Func<TSource, YieldAll<TSource>> yieldAll)
    {
        foreach (var item in source)
        {
            yield return item;
        }

        foreach (var item in yieldAll(default!).Source)
        {
            yield return item;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [ComputationBuilderTargetType(typeof(AsType<,>))]
    public static TResult Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, AsType<TSource, TResult>> asType) =>
        asType(default!).Constructor(source);
}
