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
            if (invocation == null)
            {
                return;
            }

            // Verify we're dealing with a string.Format() call
            if (!invocation.IsAnInvocationOf(typeof(string), nameof(string.Format), context.SemanticModel))
            {
                return;
            }

            if (invocation.ArgumentList == null)
            {
                return;
            }

            // Verify the format is a literal expression and not a method invocation or an identifier
            // The overloads are in the form string.Format(string, object[]) or string.Format(CultureInfo, string, object[])
            var allArguments = invocation.ArgumentList.Arguments;
            var firstArgument = allArguments.ElementAtOrDefault(0, null);
            var secondArgument = allArguments.ElementAtOrDefault(1, null);
            if (firstArgument == null)
            {
                return;
            }

            var firstArgumentIsLiteral = firstArgument.Expression is LiteralExpressionSyntax;
            var secondArgumentIsLiteral = secondArgument?.Expression is LiteralExpressionSyntax;
            if (!firstArgumentIsLiteral && !secondArgumentIsLiteral)
            {
                return;
            }

            // We ignore interpolated strings for now (workitem tracked in https://github.com/Vannevelj/VSDiagnostics/issues/313)
            if (firstArgument.Expression is InterpolatedStringExpressionSyntax)
            {
                return;
            }

            // If we got here, it means that the either the first or the second argument is a literal. 
            // If the first is a literal then that is our format
            var formatString = firstArgumentIsLiteral
                ? ((LiteralExpressionSyntax) firstArgument.Expression).GetText().ToString()
                : ((LiteralExpressionSyntax) secondArgument.Expression).GetText().ToString();

            // Get the total amount of arguments passed in for the format
            // If the first one is the literal (aka: the format specified) then every other argument is an argument to the format
            // If not, it means the first one is the CultureInfo, the second is the format and all others are format arguments
            // We also have to check whether or not the arguments are passed in through an explicit array or whether they use the params syntax
            var formatArguments = firstArgumentIsLiteral
                ? allArguments.Skip(1).ToArray()
                : allArguments.Skip(2).ToArray();
            var amountOfFormatArguments = formatArguments.Length;

            if (formatArguments.Length == 1)
            {
                // Inline array creation à la string.Format("{0}", new object[] { "test" })
                var arrayCreation = formatArguments[0].Expression as ArrayCreationExpressionSyntax;
                if (arrayCreation?.Initializer?.Expressions != null)
                {
                    amountOfFormatArguments = arrayCreation.Initializer.Expressions.Count;
                }

                // We don't handle method calls
                var invocationExpression = formatArguments[0].Expression as InvocationExpressionSyntax;
                if (invocationExpression != null)
                {
                    return;
                }

                // If it's an identifier, we don't handle those that provide an array as a single argument
                // Other types are fine though -- think about string.Format("{0}", name);
                var referencedIdentifier = formatArguments[0].Expression as IdentifierNameSyntax;
                if (referencedIdentifier != null)
                {
                    // This is also hit by any other kind of identifier so we have to differentiate
                    var referencedType = context.SemanticModel.GetTypeInfo(referencedIdentifier);
                    if (referencedType.Type == null || referencedType.Type is IErrorTypeSymbol)
                    {
                        return;
                    }

                    if (referencedType.Type.TypeKind.HasFlag(SyntaxKind.ArrayType))
                    {
                        // If we got here it means the arguments are passed in through an identifier which resolves to an array 
                        // aka: calling a method that returns an array or referencing a variable/field that is of type array
                        // We cannot reliably get the amount of arguments if it's a method
                        // We could get them when it's a field/variable/property but that takes some more work and thinking about it
                        // This is tracked in workitem https://github.com/Vannevelj/VSDiagnostics/issues/330
                        return;
                    }
                }
            }

            // Get the placeholders we use, stripped off their format specifier, get the highest value 
            // and verify that this value + 1 (to account for 0-based indexing) is not greater than the amount of placeholder arguments
            var placeholders = PlaceholderHelpers.GetPlaceholders(formatString)
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
                context.ReportDiagnostic(Diagnostic.Create(Rule, firstArgumentIsLiteral
                    ? firstArgument.GetLocation()
                    : secondArgument.GetLocation()));
            }
        }
    }
}