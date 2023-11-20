#pragma warning disable CS9113 // Parameter is unread.
namespace Kehlet.Functional;

[AttributeUsage(AttributeTargets.Class)]
public class AutoClosedAttribute(bool serializable = false) : Attribute
{
}
