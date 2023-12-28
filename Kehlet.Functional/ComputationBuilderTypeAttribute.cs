namespace Kehlet.Functional;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ComputationBuilderTypeAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class ComputationBuilderTargetTypeAttribute(Type type) : Attribute;
