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
            int amountOfArguments;
            if (firstArgumentIsLiteral)
            {
                amountOfArguments = allArguments.Count - 1;
            }
            else
            {
                amountOfArguments = allArguments.Count - 2;
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
            if (highestPlaceholder + 1 > amountOfArguments)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, firstArgumentIsLiteral ? firstArgument.GetLocation() : 
                                                                                          secondArgument.GetLocation()));
            }
        }
    }
}