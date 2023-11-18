using ExhaustiveMatching;

namespace Kehlet.Functional;

[AutoClosed]
public partial record OptionUnion<TValue>
{
    partial record Some(TValue Value);

    partial record None;
}
