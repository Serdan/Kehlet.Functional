namespace Kehlet.Functional;

static partial class Prelude
{
    [Pure]
    public static Func<T1, T3> compose<T1, T2, T3>(
        Func<T1, T2>f1,
        Func<T2, T3>f2) =>
        x => f2(f1(x));

    [Pure]
    public static Func<T1, T4> compose<T1, T2, T3, T4>(
        Func<T1, T2>f1,
        Func<T2, T3>f2,
        Func<T3, T4>f3) =>
        x => f3(f2(f1(x)));

    [Pure]
    public static Func<T1, T5> compose<T1, T2, T3, T4, T5>(
        Func<T1, T2>f1,
        Func<T2, T3>f2,
        Func<T3, T4>f3,
        Func<T4, T5>f4) =>
        x => f4(f3(f2(f1(x))));

    [Pure]
    public static Func<T1, T6> compose<T1, T2, T3, T4, T5, T6>(
        Func<T1, T2>f1,
        Func<T2, T3>f2,
        Func<T3, T4>f3,
        Func<T4, T5>f4,
        Func<T5, T6>f5) =>
        x => f5(f4(f3(f2(f1(x)))));

    [Pure]
    public static Func<T1, T7> compose<T1, T2, T3, T4, T5, T6, T7>(
        Func<T1, T2>f1,
        Func<T2, T3>f2,
        Func<T3, T4>f3,
        Func<T4, T5>f4,
        Func<T5, T6>f5,
        Func<T6, T7>f6) =>
        x => f6(f5(f4(f3(f2(f1(x))))));

    [Pure]
    public static Func<T1, T8> compose<T1, T2, T3, T4, T5, T6, T7, T8>(
        Func<T1, T2>f1,
        Func<T2, T3>f2,
        Func<T3, T4>f3,
        Func<T4, T5>f4,
        Func<T5, T6>f5,
        Func<T6, T7>f6,
        Func<T7, T8>f7) =>
        x => f7(f6(f5(f4(f3(f2(f1(x)))))));

}
