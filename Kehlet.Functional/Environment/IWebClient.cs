using Kehlet.Generators;

namespace Kehlet.Functional;

[FromInstanceMembers(typeof(HttpClient), implement: true, voidType: typeof(Unit))]
public partial interface IWebIO;

public readonly struct WebIO(HttpClient client) : IWebIO
{
    public HttpClient Instance => client;
}

public interface IHasWeb<TRuntime>
    where TRuntime : struct, IHasWeb<TRuntime>
{
    static abstract AsyncEffect<TRuntime, IWebIO> Web { get; }
}
