using System.Threading.Tasks;
using Xunit;

namespace Kehlet.Functional.Analyzers.Tests;

using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MonadicTargetTypeAnalyzer>;

public class SampleSemanticAnalyzerTests
{
    [Fact]
    public async Task SetSpeedHugeSpeedSpecified_AlertDiagnostic()
    {
        const string text = @"
using static Kehlet.Functional.Prelude;

public class Program
{
    public void Main()
    {
        var spaceship = new Spaceship();
        spaceship.SetSpeed(300000000);
    }
}

public static class Examples
{
    public static void Do()
    {
        var b = from _ in some(1)
                select orElse(2) into _
                select orElse("""");
    }
}
";

        var expected = Verifier.Diagnostic()
                               .WithLocation(7, 28)
                               .WithArguments("300000000");
        await Verifier.VerifyAnalyzerAsync(text, expected);
    }
}
