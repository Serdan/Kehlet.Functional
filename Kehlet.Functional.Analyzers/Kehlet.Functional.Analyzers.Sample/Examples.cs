namespace Kehlet.Functional.Analyzers.Sample;

using static Prelude;

public static class Examples
{
    public static void Do()
    {
        var asd = from a in some(1)
                  select orElse(2) into _
                  select orElse(2) into _
                  select orElse(2) into _
                  select orElse(2) into _
                  select orElse(2) into _
                  select orElse(2) into _
                  select orElse(2) into _
                  select orElse(2);
    }
}
