using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Kehlet.Functional.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MonadicTargetTypeAnalyzer : DiagnosticAnalyzer
{
    // Preferred format of DiagnosticId is Your Prefix + Number, e.g. CA1234.
    private const string DiagnosticId = "KEHFUN0001";

    // Feel free to use raw strings if you don't need localization.
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.KEHFUN0001Title),
                                                                                    Resources.ResourceManager, typeof(Resources));

    // The message that will be displayed to the user.
    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.KEHFUN0001MessageFormat), Resources.ResourceManager,
                                      typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.KEHFUN0001Description), Resources.ResourceManager,
                                      typeof(Resources));

    // The category of the diagnostic (Design, Naming etc.).
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
                                                            DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    // Keep in mind: you have to list your rules here.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // You must call this method to avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        // You must call this method to enable the Concurrent Execution.
        context.EnableConcurrentExecution();

        // Subscribe to semantic (compile time) action invocation, e.g. method invocation.
        context.RegisterOperationAction(AnalyzeQuery, OperationKind.TranslatedQuery);
    }

    private static void AnalyzeQuery(OperationAnalysisContext context)
    {
        if (context.Operation is not ITranslatedQueryOperation operation)
        {
            return;
        }

        var visitor = new SelectInvocationVisitor();
        operation.Accept(visitor);

        foreach (var visitorOperation in visitor.Operations)
        {
            var diagnostic = Diagnostic.Create(Rule, visitorOperation.Operation.Syntax.GetLocation(), visitorOperation.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}

public class SelectInvocationVisitor : OperationWalker
{
    public List<DiagnosticDetails> Operations { get; } = [];

    public override void VisitInvocation(IInvocationOperation operation)
    {
        Core();

        base.VisitInvocation(operation);
        return;

        void Core()
        {
            var methodSymbol = operation.TargetMethod;
            switch (methodSymbol.Name)
            {
                case "Select":
                    HandleSelect(methodSymbol);
                    break;
                case "SelectMany":
                    HandleSelectMany(methodSymbol);
                    break;
            }
        }

        void HandleSelect(IMethodSymbol methodSymbol)
        {
            var funcArgument = operation.Arguments.LastOrDefault();

            if (funcArgument?.Value.Type is not INamedTypeSymbol funcType)
            {
                return;
            }

            if (funcType.ConstructedFrom is not { Name: "Func", ContainingNamespace.Name: "System" })
            {
                return;
            }

            var resultType = funcType.TypeArguments[1] as INamedTypeSymbol;
            var isBuilderType = resultType?.GetAttributes()
                                          .Any(x => x.AttributeClass?.Name is "ComputationBuilderTypeAttribute");

            if (resultType is null || isBuilderType is false)
            {
                return;
            }

            var methodAttributes = methodSymbol.GetAttributes();
            var targetTypes = methodAttributes.Where(x => x.AttributeClass?.Name is "ComputationBuilderTargetTypeAttribute")
                                              .Select(x => x.ConstructorArguments.First().Value as INamedTypeSymbol)
                                              .Where(x => x is not null)
                                              .ToArray();

            if (targetTypes.Select(x => x!.ConstructedFrom).Contains(resultType.ConstructedFrom, SymbolEqualityComparer.Default))
            {
                return;
            }

            // A target type is used in a method that does not support it.
            Operations.Add(new(operation, resultType.ConstructedFrom.Name));
        }

        void HandleSelectMany(IMethodSymbol methodSymbol)
        {
            
        }
    }
}

public record DiagnosticDetails(IInvocationOperation Operation, string Name);
