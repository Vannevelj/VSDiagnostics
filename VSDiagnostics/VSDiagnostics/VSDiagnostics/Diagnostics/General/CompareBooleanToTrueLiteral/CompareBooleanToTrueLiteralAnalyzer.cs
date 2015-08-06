using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompareBooleanToTrueLiteralAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(CompareBooleanToTrueLiteralAnalyzer);
        private const string Message = "A boolean expression can be simplified.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "A boolean expression doesn't have to be compared to true.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.TrueLiteralExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var equalsExpression = context.Node as LiteralExpressionSyntax;
            if (equalsExpression == null)
            {
                return;
            }

            if (!(equalsExpression.Token.IsKind(SyntaxKind.TrueKeyword) && equalsExpression.Token.Value is bool))
            {
                return;
            }

            var parentExpression = equalsExpression.Parent as BinaryExpressionSyntax;
            if (parentExpression == null)
            {
                return;
            }

            if (!parentExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, equalsExpression.GetLocation()));
        }
    }
}