using Kehlet.Generators;

namespace Kehlet.Functional;

[FromInstanceMembers(typeof(HttpClient), implement: true, voidType: typeof(Unit))]
public partial interface IWebIO;

[UsedImplicitly]
public readonly struct WebIO(HttpClient client) : IWebIO
{
    public HttpClient Instance => client;
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IHasWeb<TRuntime>
    where TRuntime : struct, IHasWeb<TRuntime>
{
    AsyncEffect<TRuntime, IWebIO> Web { get; }
}
