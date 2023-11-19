namespace Kehlet.Functional;

[AttributeUsage(AttributeTargets.Class)]
public class AutoClosedAttribute(bool serializable = false) : Attribute
{
}
