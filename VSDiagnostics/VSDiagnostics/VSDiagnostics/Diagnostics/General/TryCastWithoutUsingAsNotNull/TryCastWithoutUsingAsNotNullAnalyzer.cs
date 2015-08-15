using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TryCastWithoutUsingAsNotNullAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(TryCastWithoutUsingAsNotNullAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.TryCastWithoutUsingAsNotNullAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.TryCastWithoutUsingAsNotNullAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

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

            var ifExpression = isExpression.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
            if (ifExpression == null)
            {
                return;
            }

            var asExpressions = ifExpression.Statement.DescendantNodes().OfType<BinaryExpressionSyntax>().Where(x => x.OperatorToken.Kind() == SyntaxKind.AsKeyword);
            foreach (var asExpression in asExpressions)
            {
                var asIdentifier = asExpression.Left as IdentifierNameSyntax;
                if (asIdentifier == null)
                {
                    continue;
                }

                if (!string.Equals(asIdentifier.Identifier.ValueText, isIdentifier))
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(Rule, isExpression.GetLocation(), isIdentifier));
            }

            var castExpressions = ifExpression.Statement.DescendantNodes().OfType<CastExpressionSyntax>().ToArray();
            foreach (var castExpression in castExpressions)
            {
                var castIdentifier = castExpression.Expression as IdentifierNameSyntax;
                if (castIdentifier == null)
                {
                    continue;
                }

                if (!string.Equals(castIdentifier.Identifier.ValueText, isIdentifier))
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(Rule, isExpression.GetLocation(), isIdentifier));
            }
        }
    }
}