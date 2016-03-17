using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.CastToAs
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CastToAsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.CastToAsAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.CastToAsAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.CastToAs, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.CastExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax) context.Node;

            var castedTypeInfo = context.SemanticModel.GetTypeInfo(castExpression.Expression);
            if (castedTypeInfo.ConvertedType != null && castedTypeInfo.ConvertedType.IsValueType)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, castExpression.GetLocation()));
        }
    }
}