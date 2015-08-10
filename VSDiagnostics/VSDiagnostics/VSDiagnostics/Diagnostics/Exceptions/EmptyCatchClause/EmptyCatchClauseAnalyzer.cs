using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Exceptions.EmptyCatchClause
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyCatchClauseAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(EmptyCatchClause);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;
        private static readonly string Message = VSDiagnosticsResources.EmptyCatchClauseAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.EmptyCatchClauseAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CatchClause);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;
            if (catchClause == null)
            {
                return;
            }

            var statements = catchClause.Block?.Statements;
            if (statements.HasValue && statements.Value.Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, catchClause.CatchKeyword.GetLocation()));
        }
    }
}