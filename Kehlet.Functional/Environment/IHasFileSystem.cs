namespace Kehlet.Functional;

[UsedImplicitly]
public interface IHasFileSystem<TRuntime> : IHasFile<TRuntime>, IHasDirectory<TRuntime>
    where TRuntime : struct, IHasFile<TRuntime>, IHasDirectory<TRuntime>;
