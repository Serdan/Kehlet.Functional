using Kehlet.Generators;

namespace Kehlet.Functional;

[FromStaticMembers(typeof(Directory), implement: true, voidType: typeof(Unit))]
public partial interface IDirectoryIO;

[UsedImplicitly]
public readonly struct DirectoryIO : IDirectoryIO;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IHasDirectory<TRuntime>
    where TRuntime : struct, IHasDirectory<TRuntime>
{
    Effect<TRuntime, IDirectoryIO> Directory { get; }
}
