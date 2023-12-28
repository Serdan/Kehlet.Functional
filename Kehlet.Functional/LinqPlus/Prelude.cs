using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Kehlet.Functional.LinqPlus;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static partial class Prelude
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static First first() =>
        new(true);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static First first(bool predicate) =>
        new(predicate);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Last last() =>
        new(true);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Last last(bool predicate) =>
        new(predicate);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Take take(int count) =>
        new(count);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeRange take(Range range) =>
        new(range);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeWhile takeWhile(bool predicate) =>
        new(predicate);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Reverse reverse() =>
        new();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Distinct distinct() =>
        new();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DistinctBy<TKey> distinct<TKey>(TKey key) =>
        new(key);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static YieldAll<TSource> yieldAll<TSource>(IEnumerable<TSource> source) =>
        new(source);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AsType<TSource, TResult> asType<TSource, TResult>(Func<IEnumerable<TSource>, TResult> constructor) =>
        new(constructor);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Run run() =>
        new();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Run<TRuntime> run<TRuntime>(TRuntime runtime) =>
        new(runtime);

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public readonly struct Fold<TAccumulate>(TAccumulate initialValue)
    {
        private readonly Func<TAccumulate, bool>? predicate;

        private Fold(TAccumulate initialValue, Func<TAccumulate, bool> predicate) : this(initialValue) =>
            this.predicate = predicate;

        [Pure]
        public Fold<TAccumulate> Where(Func<TAccumulate, bool> predicate) =>
            new(initialValue, predicate);

        [Pure]
        public TAccumulate SelectMany<TValue>(Func<TAccumulate, IEnumerable<TValue>> selector, Func<TAccumulate, TValue, TAccumulate> accumulator)
        {
            var values = selector(initialValue);

            var acc = initialValue;
            foreach (var value in values)
            {
                acc = accumulator(acc, value);
                if (predicate?.Invoke(acc) is false)
                {
                    return acc;
                }
            }

            return acc;
        }
    }
}
