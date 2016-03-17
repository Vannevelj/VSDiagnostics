using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToFalseLiteral
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompareBooleanToFalseLiteralAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.CompareBooleanToFalseLiteralAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.CompareBooleanToFalseLiteralAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.CompareBooleanToFalseLiteral, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FalseLiteralExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = (LiteralExpressionSyntax) context.Node;

            if (!(literalExpression.Token.IsKind(SyntaxKind.FalseKeyword) && literalExpression.Token.Value is bool))
            {
                return;
            }

            var binaryExpression = literalExpression.Parent as BinaryExpressionSyntax;
            if (binaryExpression == null)
            {
                return;
            }

            if (binaryExpression.Left == literalExpression)
            {
                // Check the right-hand side
                var rightSymbol = context.SemanticModel.GetTypeInfo(binaryExpression.Right);
                if (rightSymbol.Type == null)
                {
                    return;
                }

                if (rightSymbol.Type.IsNullable())
                {
                    return;
                }
            }
            else
            {
                // Check the left-hand side
                var leftSymbol = context.SemanticModel.GetTypeInfo(binaryExpression.Left);
                if (leftSymbol.Type == null)
                {
                    return;
                }

                if (leftSymbol.Type.IsNullable())
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, literalExpression.GetLocation()));
        }
    }
}