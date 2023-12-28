using System.Threading.Tasks;
using Kehlet.Functional.Extensions;

namespace Kehlet.Functional.Analyzers.Sample;

using static Prelude;

public static class Examples
{
    public static async Task Do()
    {
        // var asd = from a in some(1)
        //           select orElse(2) into _
        //           select orElse(2) into _
        //           select orElse(2) into _
        //           select orElse(2) into _
        //           select orElse(2) into _
        //           select orElse(2) into _
        //           select orElse(2) into _
        //           select orElse(2);

        Result<int> wat = await from eff1 in Task.FromResult(effect(() => ok(1)))
                                from eff2 in Task.FromResult(effect(() => ok(2)))
                                select from v1 in eff1
                                       from v2 in eff2
                                       select v1 + v2 into r
                                       select run();
    }
}
