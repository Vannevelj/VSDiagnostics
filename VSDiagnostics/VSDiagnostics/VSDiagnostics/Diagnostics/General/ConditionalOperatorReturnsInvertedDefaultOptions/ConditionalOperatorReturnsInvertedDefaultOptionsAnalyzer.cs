using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsInvertedDefaultOptions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;

        private static readonly string Message =
            VSDiagnosticsResources.ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzerMessage;

        private static readonly string Title =
            VSDiagnosticsResources.ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ConditionalOperatorReturnsInvertedDefaultOptions, Title, Message, Category, Severity, true);

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

            var hasInvertedTrueLiteral = trueExpression.Token.IsKind(SyntaxKind.FalseKeyword) &&
                                         trueExpression.Token.Value is bool;
            var hasInvertedFalseLiteral = falseExpression.Token.IsKind(SyntaxKind.TrueKeyword) &&
                                          falseExpression.Token.Value is bool;

            if (hasInvertedTrueLiteral && hasInvertedFalseLiteral)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, conditionalExpression.GetLocation()));
            }
        }
    }
}