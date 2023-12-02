namespace Kehlet.Functional;

[Union]
public partial record ResultUnion<TValue>
{
    partial record Ok(TValue Value);
    
    partial record Error(Exception Exception);
}
