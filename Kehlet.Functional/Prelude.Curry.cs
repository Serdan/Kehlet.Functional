namespace Kehlet.Functional;

static partial class Prelude
{
    [Pure]
    public static Func<T1, Func<T2, T3>> curry<T1, T2, T3>(Func<T1, T2, T3> f) =>
        t1 => t2 => f(t1, t2);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, T4>>> curry<T1, T2, T3, T4>(Func<T1, T2, T3, T4> f) =>
        t1 => t2 => t3 => f(t1, t2, t3);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, T5>>>> curry<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> f) =>
        t1 => t2 => t3 => t4 => f(t1, t2, t3, t4);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, T6>>>>> curry<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> f) =>
        t1 => t2 => t3 => t4 => t5 => f(t1, t2, t3, t4, t5);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, T7>>>>>> curry<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7> f) =>
        t1 => t2 => t3 => t4 => t5 => t6 => f(t1, t2, t3, t4, t5, t6);

    [Pure]
    public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, T8>>>>>>> curry<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8> f) =>
        t1 => t2 => t3 => t4 => t5 => t6 => t7 => f(t1, t2, t3, t4, t5, t6, t7);

    [Pure]
    public static Func<T1, Func<T2, T3, T4>> curry2<T1, T2, T3, T4>(Func<T1, T2, T3, T4> f) =>
        t1 => (t2, t3) => f(t1, t2, t3);

    [Pure]
    public static Func<T1, Func<T2, T3, T4, T5>> curry2<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> f) =>
        t1 => (t2, t3, t4) => f(t1, t2, t3, t4);

    [Pure]
    public static Func<T1, Func<T2, T3, T4, T5, T6>> curry2<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> f) =>
        t1 => (t2, t3, t4, t5) => f(t1, t2, t3, t4, t5);

    [Pure]
    public static Func<T1, Func<T2, T3, T4, T5, T6, T7>> curry2<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7> f) =>
        t1 => (t2, t3, t4, t5, t6) => f(t1, t2, t3, t4, t5, t6);

    [Pure]
    public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8>> curry2<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8> f) =>
        t1 => (t2, t3, t4, t5, t6, t7) => f(t1, t2, t3, t4, t5, t6, t7);


}
