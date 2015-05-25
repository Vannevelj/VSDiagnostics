using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsDefaultOptions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalOperatorReturnsDefaultOptionsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(ConditionalOperatorReturnsDefaultOptionsAnalyzer);
        internal const string Title = "The conditional operator shouldn't return just literals.";
        internal const string Message = "A conditional operator can be omitted.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ConditionalExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = context.Node as ConditionalExpressionSyntax;
            if (conditionalExpression == null)
            {
                return;
            }

            var trueExpression = conditionalExpression.WhenTrue as LiteralExpressionSyntax;
            if (trueExpression == null)
            {
                return;
            }

            var falseExpression = conditionalExpression.WhenFalse as LiteralExpressionSyntax;
            if (falseExpression == null)
            {
                return;
            }

            var hasTrueLiteral = trueExpression.Token.ValueText == "true";
            var hasFalseLiteral = falseExpression.Token.ValueText == "false";

            if (hasTrueLiteral && hasFalseLiteral)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, conditionalExpression.GetLocation()));
            }
        }
    }
}