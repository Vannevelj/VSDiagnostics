using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TryCastWithoutUsingAsNotNullAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.TryCastWithoutUsingAsNotNullAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.TryCastWithoutUsingAsNotNullAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.TryCastWithoutUsingAsNotNull, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IsExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var isExpression = context.Node as BinaryExpressionSyntax;

            var isIdentifierExpression = isExpression?.Left as IdentifierNameSyntax;
            if (isIdentifierExpression == null)
            {
                return;
            }
            var isIdentifier = isIdentifierExpression.Identifier.ValueText;

            var ifStatement = isExpression.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
            if (ifStatement == null)
            {
                return;
            }

            var asExpressions = ifStatement.Statement
                                           .DescendantNodes()
                                           .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                           .OfType<BinaryExpressionSyntax>()
                                           .Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword));

            var castExpressions = ifStatement.Statement
                                             .DescendantNodes()
                                             .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                             .OfType<CastExpressionSyntax>();

            Action<string> checkIdentifier = bodyIdentifier =>
            {
                if (bodyIdentifier == isIdentifier)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, isExpression.GetLocation(), isIdentifier));
                }
            };

            foreach (var expression in asExpressions.Concat<ExpressionSyntax>(castExpressions))
            {
                var binaryExpression = expression as BinaryExpressionSyntax;
                var binaryIdentifier = binaryExpression?.Left as IdentifierNameSyntax;
                if (binaryIdentifier != null)
                {
                    checkIdentifier(binaryIdentifier.Identifier.ValueText);
                    continue;
                }

                var castExpression = expression as CastExpressionSyntax;
                var castIdentifier = castExpression?.Expression as IdentifierNameSyntax;
                if (castIdentifier != null)
                {
                    checkIdentifier(castIdentifier.Identifier.ValueText);
                }
            }
        }
    }
}