namespace Kehlet.Functional;

[ComputationBuilderType]
public readonly record struct Run;

[ComputationBuilderType]
public readonly record struct Run<TRuntime>(TRuntime runtime);
