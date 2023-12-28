namespace Kehlet.Functional;

[ComputationBuilderType]
public readonly record struct OrElse<T>(Option<T> Option)
    where T : notnull;
