using System.Collections.Immutable;
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

            // Verify we're dealing with a string.Format call

            // Verify the format is a literal expression and not a method invocation or an identifier


        }
    }
}