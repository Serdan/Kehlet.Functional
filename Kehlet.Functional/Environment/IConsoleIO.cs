using Kehlet.Generators;

namespace Kehlet.Functional;

[FromStaticMembers(typeof(Console), true, typeof(Unit))]
public partial interface IConsoleIO;

[UsedImplicitly]
public readonly struct ConsoleIO : IConsoleIO;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IHasConsole<TRuntime>
    where TRuntime : struct, IHasConsole<TRuntime>
{
    Effect<TRuntime, IConsoleIO> Console { get; }
}
