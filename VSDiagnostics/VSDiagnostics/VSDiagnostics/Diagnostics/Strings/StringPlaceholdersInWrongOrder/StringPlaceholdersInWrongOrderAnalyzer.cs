using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Strings.StringPlaceholdersInWrongOrder
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringPlaceholdersInWrongOrderAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Strings";
        private const string DiagnosticId = nameof(StringPlaceholdersInWrongOrderAnalyzer);
        private const string Message = "Placeholders are not in ascending order.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Orders the arguments of a string.Format() call in ascending order according to index.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

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
            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression != null)
            {
                var invokedType = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression);
                var invokedMethod = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Name);
                if (invokedType.Symbol == null || invokedMethod.Symbol == null)
                {
                    return;
                }

                if (invokedType.Symbol.MetadataName != typeof(string).Name || invokedMethod.Symbol.MetadataName != nameof(string.Format))
                {
                    return;
                }
            }

            // Verify the format is a literal expression and not a method invocation or an identifier
            // The overloads are in the form string.Format(string, object[]) or string.Format(CultureInfo, string, object[])
            if (invocation.ArgumentList == null || !invocation.ArgumentList.Arguments.Any())
            {
                return;
            }

            var firstArgument = invocation.ArgumentList.Arguments[0];
            var secondArgument = invocation.ArgumentList.Arguments[1];

            var firstArgumentSymbol = context.SemanticModel.GetSymbolInfo(firstArgument.Expression);
            if (!(firstArgument.Expression is LiteralExpressionSyntax) &&
                (firstArgumentSymbol.Symbol?.MetadataName == typeof(CultureInfo).Name && !(secondArgument?.Expression is LiteralExpressionSyntax)))
            {
                return;
            }

            // Get the formatted string from the correct position
            var formattedString = firstArgument.Expression is LiteralExpressionSyntax
                ? ((LiteralExpressionSyntax) firstArgument.Expression).GetText().ToString()
                : ((LiteralExpressionSyntax) secondArgument.Expression).GetText().ToString();

            // Verify that all placeholders are counting from low to high.
            // Not all placeholders have to be used necessarily, we only re-order the ones that are actually used in the format string.
            //
            // Get all elements in a string that are enclosed by an uneven amount of curly brackets (to account for escaped brackets).
            // The result will be elements that are either plain integers or integers with a format appended to it, delimited by a colon.
            // Display a warning when the integers in question are not in ascending order. 
            var pattern = @"(?<!\{)\{(?:\{\{)*(\d+(?::.*?)?)\}(?:\}\})*(?!\})";
            var placeholders = Regex.Matches(formattedString, pattern);

            // If there's no placeholder used or there's only one, there's nothing to re-order
            if (placeholders.Count <= 1)
            {
                return;
            }

            for (var index = 1; index < placeholders.Count; index++)
            {
                int firstValue, secondValue;
                if (!int.TryParse(Normalize(placeholders[index - 1].Value), out firstValue) ||
                    !int.TryParse(Normalize(placeholders[index].Value), out secondValue))
                {
                    // Parsing failed
                    return;
                }

                // They should be in ascending or equal order
                if (firstValue > secondValue)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
                    return;
                }
            }
        }

        /// <summary>
        ///     Removes all curly braces and formatting definitions from the placeholder
        /// </summary>
        /// <param name="input">The placeholder entry to parse.</param>
        /// <returns>Returns the placeholder index.</returns>
        private static string Normalize(string input)
        {
            var temp = input.Trim('{', '}');
            var colonIndex = temp.IndexOf(':');
            if (colonIndex > 0)
            {
                return temp.Remove(colonIndex);
            }

            return temp;
        }
    }
}