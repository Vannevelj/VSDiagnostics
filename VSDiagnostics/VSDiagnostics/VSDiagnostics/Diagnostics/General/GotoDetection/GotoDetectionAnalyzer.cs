using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.GotoDetection
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class GotoDetectionAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(GotoDetectionAnalyzer);
        private const string Message = "Use of \"goto\" detected.  Consider using a method or loop instead.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;
        private const string Title = "Use of \"goto\" detected.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.GotoStatement, SyntaxKind.GotoCaseStatement, SyntaxKind.GotoDefaultStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = context.Node as GotoStatementSyntax;
            if (literalExpression == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, literalExpression.GetLocation()));
        }
    }
}