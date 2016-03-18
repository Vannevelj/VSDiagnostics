using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.GotoDetection
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GotoDetectionAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.GotoDetectionAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.GotoDetectionAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.GotoDetection, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.GotoStatement, SyntaxKind.GotoCaseStatement,
            SyntaxKind.GotoDefaultStatement);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = (GotoStatementSyntax) context.Node;

            context.ReportDiagnostic(Diagnostic.Create(Rule, literalExpression.GetLocation()));
        }
    }
}