using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
// ReSharper disable LoopCanBeConvertedToQuery

namespace VSDiagnostics.Diagnostics.Exceptions.EmptyCatchClause
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyCatchClauseAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;
        private static readonly string Message = VSDiagnosticsResources.EmptyCatchClauseAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.EmptyCatchClauseAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.EmptyCatchClause, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CatchClause);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax) context.Node;
            if (catchClause.Block == null)
            {
                return;
            }

            if (catchClause.Block.Statements.Any())
            {
                return;
            }

            foreach (var trivia in catchClause.Block.CloseBraceToken.LeadingTrivia)
            {
                if (trivia.IsCommentTrivia())
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, catchClause.CatchKeyword.GetLocation()));
        }
    }
}