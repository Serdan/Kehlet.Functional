using System.Diagnostics;
using Kehlet.Generators;

namespace Kehlet.Functional;

[FromInstanceMembers(typeof(Process), implement: true, typeof(Unit))]
public partial interface IProcessIO;

public readonly struct ProcessIO(Process process) : IProcessIO
{
    public Process Instance { get; } = process;

    public override string ToString() =>
        Instance.ToString();
}

public interface IHasProcessIO<TRuntime>
    where TRuntime : struct, IHasProcessIO<TRuntime>
{
    Effect<IProcessIO> ProcessIO(ProcessStartInfo info) =>
        Process.Start(info) is { } process
            ? okEffect((IProcessIO) new ProcessIO(process))
            : errorEffect<IProcessIO>("Failed to start process.");
}
