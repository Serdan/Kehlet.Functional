using Kehlet.Generators;

namespace Kehlet.Functional;

[FromStaticMembers(typeof(File), implement: true, voidType: typeof(Unit))]
public partial interface IFileIO;

public readonly struct FileIO : IFileIO;

public interface IHasFile<TRuntime>
    where TRuntime : struct, IHasFile<TRuntime>
{
    static abstract AsyncEffect<TRuntime, IFileIO> File { get; }
}
