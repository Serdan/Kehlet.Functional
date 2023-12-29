using Kehlet.Generators;

namespace Kehlet.Functional;

[FromStaticMembers(typeof(File), implement: true, voidType: typeof(Unit))]
public partial interface IFileIO;

[UsedImplicitly]
public readonly struct FileIO : IFileIO;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IHasFile<TRuntime>
    where TRuntime : struct, IHasFile<TRuntime>
{
    AsyncEffect<TRuntime, IFileIO> File { get; }
}
