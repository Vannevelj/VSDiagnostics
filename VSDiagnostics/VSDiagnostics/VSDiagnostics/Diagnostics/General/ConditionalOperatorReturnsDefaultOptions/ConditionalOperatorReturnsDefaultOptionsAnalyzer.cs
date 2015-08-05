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
        private const string Category = "General";
        private const string DiagnosticId = nameof(ConditionalOperatorReturnsDefaultOptionsAnalyzer);
        private const string Message = "A conditional operator can be omitted.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "The conditional operator shouldn't return redundant true and false literals.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ConditionalExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = context.Node as ConditionalExpressionSyntax;

            var trueExpression = conditionalExpression?.WhenTrue as LiteralExpressionSyntax;
            if (trueExpression == null)
            {
                return;
            }

            var falseExpression = conditionalExpression.WhenFalse as LiteralExpressionSyntax;
            if (falseExpression == null)
            {
                return;
            }

            var hasTrueLiteral = trueExpression.Token.IsKind(SyntaxKind.TrueKeyword) && trueExpression.Token.Value is bool;
            var hasFalseLiteral = falseExpression.Token.IsKind(SyntaxKind.FalseKeyword) && falseExpression.Token.Value is bool;

            if (hasTrueLiteral && hasFalseLiteral)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, conditionalExpression.GetLocation()));
            }
        }
    }
}