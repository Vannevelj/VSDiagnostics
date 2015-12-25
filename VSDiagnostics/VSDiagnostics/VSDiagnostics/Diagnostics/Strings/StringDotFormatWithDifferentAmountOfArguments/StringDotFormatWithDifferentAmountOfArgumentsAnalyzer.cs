using System;
using System.Collections.Immutable;
using System.Globalization;
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

        private static readonly string Message =
            VSDiagnosticsResources.StringDotFormatWithDifferentAmountOfArgumentsMessage;

        private static readonly string Title = VSDiagnosticsResources.StringDotFormatWithDifferentAmountOfArgumentsTitle;

        internal static DiagnosticDescriptor Rule
            =>
                new DiagnosticDescriptor(DiagnosticId.StringDotFormatWithDifferentAmountOfArguments, Title, Message,
                    Category, Severity, true);

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

            // Get the total amount of passed in arguments for the format
        }
    }
}