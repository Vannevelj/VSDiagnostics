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
        public const string DiagnosticId = nameof(CompareBooleanToTrueLiteralAnalyzer);
        internal const string Title = "A boolean expression doesn't have to be compared to true.";
        internal const string Message = "A boolean expression can be simplified.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
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

            if (!(equalsExpression.Token.ValueText == "true" && equalsExpression.Token.Value is bool))
            {
                return;
            }

            var parentExpression = equalsExpression.Parent as BinaryExpressionSyntax;
            if (parentExpression == null)
            {
                return;
            }

            if (parentExpression.OperatorToken.ValueText != "==")
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, equalsExpression.GetLocation()));
        }
    }
}