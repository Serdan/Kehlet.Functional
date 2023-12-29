namespace Kehlet.Functional.Extensions;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class Extensions
{
    public static TResult Apply<TValue, TResult>(this TValue self, Func<TValue, TResult> f) => 
        f(self);
}
