using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.AsToCast
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsToCastAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(AsToCastAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Info;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.AsToCastAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.AsToCastAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.AsExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = context.Node as BinaryExpressionSyntax;
            if (binaryExpression == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, binaryExpression.GetLocation()));
        }
    }
}