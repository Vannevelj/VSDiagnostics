using System;
using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Strings.StringPlaceholdersInWrongOrder
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringPlaceholdersInWrongOrderAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.StringsCategory;
        private static readonly string Message = VSDiagnosticsResources.StringPlaceholdersInWrongOrderMessage;
        private static readonly string Title = VSDiagnosticsResources.StringPlaceholdersInWrongOrderTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.StringPlaceholdersInWrongOrder, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax) context.Node;

            // Verify we're dealing with a string.Format() call
            if (!invocation.IsAnInvocationOf(typeof (string), nameof(string.Format), context.SemanticModel))
            {
                return;
            }

            // Verify the format is a literal expression and not a method invocation or an identifier
            // The overloads are in the form string.Format(string, object[]) or string.Format(CultureInfo, string, object[])
            if (invocation.ArgumentList == null || invocation.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var firstArgument = invocation.ArgumentList.Arguments[0];
            var secondArgument = invocation.ArgumentList.Arguments[1];

            var firstArgumentSymbol = context.SemanticModel.GetSymbolInfo(firstArgument.Expression);
            if (!(firstArgument.Expression is LiteralExpressionSyntax) &&
                (firstArgumentSymbol.Symbol?.MetadataName == typeof (CultureInfo).Name &&
                 !(secondArgument?.Expression is LiteralExpressionSyntax)))
            {
                return;
            }

            if (!(firstArgument.Expression is LiteralExpressionSyntax) &&
                !(secondArgument.Expression is LiteralExpressionSyntax))
            {
                return;
            }

            // Get the formatted string from the correct position
            var firstArgumentIsLiteral = firstArgument.Expression is LiteralExpressionSyntax;
            var formatString = firstArgumentIsLiteral
                ? ((LiteralExpressionSyntax) firstArgument.Expression).GetText().ToString()
                : ((LiteralExpressionSyntax) secondArgument.Expression).GetText().ToString();

            // Verify that all placeholders are counting from low to high.
            // Not all placeholders have to be used necessarily, we only re-order the ones that are actually used in the format string.
            //
            // Display a warning when the integers in question are not in ascending or equal order. 
            var placeholders = PlaceholderHelpers.GetPlaceholders(formatString);

            // If there's no placeholder used or there's only one, there's nothing to re-order
            if (placeholders.Count <= 1)
            {
                return;
            }

            for (var index = 1; index < placeholders.Count; index++)
            {
                int firstValue, secondValue;
                if (!int.TryParse(placeholders[index - 1].Groups["index"].Value, out firstValue) ||
                    !int.TryParse(placeholders[index].Groups["index"].Value, out secondValue))
                {
                    // Parsing failed
                    return;
                }

                // Given a scenario like this:
                //     string.Format("{0} {1} {4} {3}", a, b, c, d)
                // it would otherwise crash because it's trying to access index 4, which we obviously don't have.
                var argumentsToSkip = firstArgumentIsLiteral ? 1 : 2;
                if (firstValue >= invocation.ArgumentList.Arguments.Count - argumentsToSkip ||
                    secondValue >= invocation.ArgumentList.Arguments.Count - argumentsToSkip)
                {
                    return;
                }

                // Given a scenario {0} {1} {0} we have to make sure that this doesn't trigger a warning when we're simply re-using an index. 
                // Those are exempt from the "always be ascending or equal" rule.
                Func<int, int, bool> hasBeenUsedBefore = (value, currentIndex) =>
                {
                    for (var counter = 0; counter < currentIndex; counter++)
                    {
                        int intValue;
                        if (int.TryParse(placeholders[counter].Groups["index"].Value, out intValue) && intValue == value)
                        {
                            return true;
                        }
                    }

                    return false;
                };

                // They should be in ascending or equal order
                if (firstValue > secondValue && !hasBeenUsedBefore(secondValue, index))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
                    return;
                }
            }
        }
    }
}