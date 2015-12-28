using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Strings.StringDotFormatWithDifferentAmountOfArguments
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringDotFormatWithDifferentAmountOfArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private static readonly string Category = VSDiagnosticsResources.StringsCategory;
        private static readonly string Message = VSDiagnosticsResources.StringDotFormatWithDifferentAmountOfArgumentsMessage;
        private static readonly string Title = VSDiagnosticsResources.StringDotFormatWithDifferentAmountOfArgumentsTitle;

        internal static DiagnosticDescriptor Rule =>
            new DiagnosticDescriptor(DiagnosticId.StringDotFormatWithDifferentAmountOfArguments, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation?.ArgumentList == null)
            {
                return;
            }

            // Get the format string
            // This corresponds to the argument passed to the parameter with name 'format'
            var invokedMethod = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = invokedMethod.Symbol as IMethodSymbol;

            // Verify we're dealing with a call to a method that accepts a variable named 'format' and a object, params object[] or a plain object[]
            // params object[] and object[] can both be verified by looking for the latter
            // This allows us to support similar calls like Console.WriteLine("{0}", "test") as well which carry an implicit string.Format
            var formatParam = methodSymbol?.Parameters.FirstOrDefault(x => x.Name == "format");
            if (formatParam == null)
            {
                return;
            }
            var formatIndex = formatParam.Ordinal;

            var formatParameters = methodSymbol.Parameters.Skip(formatIndex + 1).ToArray();
            var hasObjectArray = formatParameters.Length == 1 &&
                                 formatParameters.All(x => x.Type.Kind == SymbolKind.ArrayType &&
                                                           ((IArrayTypeSymbol) x.Type).ElementType.SpecialType == SpecialType.System_Object);
            var hasObject = formatParameters.All(x => x.Type.SpecialType == SpecialType.System_Object);

            if (!(hasObject || hasObjectArray))
            {
                return;
            }

            var formatExpression = invocation.ArgumentList.Arguments[formatIndex].Expression;
            var formatString = context.SemanticModel.GetConstantValue(formatExpression);
            if (!formatString.HasValue)
            {
                return;
            }

            // Get the total amount of arguments passed in for the format
            // If the first one is the literal (aka: the format specified) then every other argument is an argument to the format
            // If not, it means the first one is the CultureInfo, the second is the format and all others are format arguments
            // We also have to check whether or not the arguments are passed in through an explicit array or whether they use the params syntax
            var formatArguments = invocation.ArgumentList.Arguments.Skip(formatIndex + 1).ToArray();
            var amountOfFormatArguments = formatArguments.Length;

            if (formatArguments.Length == 1)
            {
                var argumentType = context.SemanticModel.GetTypeInfo(formatArguments[0].Expression);
                if (argumentType.Type == null)
                {
                    return;
                }

                // Inline array creation à la string.Format("{0}", new object[] { "test" })
                if (argumentType.Type.TypeKind == TypeKind.Array)
                {
                    // We check for an invocation first to account for the scenario where you have both an invocation and an array initializer
                    // Think about something like this: string.Format(""{0}{1}{2}"", new[] { 1 }.Concat(new[] {2}).ToArray());
                    var methodInvocation = formatArguments[0].DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
                    if (methodInvocation != null)
                    {
                        // We don't handle method calls that return an array in the case of a single argument
                        return;
                    }

                    var inlineArrayCreation = formatArguments[0].DescendantNodes().OfType<InitializerExpressionSyntax>().FirstOrDefault();
                    if (inlineArrayCreation != null)
                    {
                        amountOfFormatArguments = inlineArrayCreation.Expressions.Count;
                        goto placeholderVerification;
                    }

                    // If we got here it means the arguments are passed in through an identifier which resolves to an array 
                    // aka: referencing a variable/field that is of type T[]
                    // We cannot reliably get the amount of arguments if it's a method
                    // We could get them when it's a field/variable/property but that takes some more work and thinking about it
                    // This is tracked in workitem https://github.com/Vannevelj/VSDiagnostics/issues/330
                    if (hasObjectArray)
                    {
                        return;
                    }
                }
            }

            placeholderVerification:
            // Get the placeholders we use stripped off their format specifier, get the highest value 
            // and verify that this value + 1 (to account for 0-based indexing) is not greater than the amount of placeholder arguments
            var placeholders = PlaceholderHelpers.GetPlaceholders((string) formatString.Value)
                                                 .Cast<Match>()
                                                 .Select(x => x.Value)
                                                 .Select(PlaceholderHelpers.GetPlaceholderIndex)
                                                 .Select(int.Parse)
                                                 .ToList();

            if (!placeholders.Any())
            {
                return;
            }

            var highestPlaceholder = placeholders.Max();
            if (highestPlaceholder + 1 > amountOfFormatArguments)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, formatExpression.GetLocation()));
            }
        }
    }
}