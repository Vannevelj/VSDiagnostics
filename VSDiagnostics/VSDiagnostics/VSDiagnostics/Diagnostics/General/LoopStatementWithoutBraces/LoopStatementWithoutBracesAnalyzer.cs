using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.LoopStatementWithoutBraces
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LoopStatementWithoutBracesAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.LoopStatementWithoutBracesAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.LoopStatementWithoutBracesAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.LoopStatementWithoutBraces, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ForStatement, SyntaxKind.ForEachStatement,
                SyntaxKind.WhileStatement, SyntaxKind.DoStatement);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var forLoop = context.Node as ForStatementSyntax;
            if (forLoop != null)
            {
                HandleFor(context, forLoop);
                return;
            }

            var whileLoop = context.Node as WhileStatementSyntax;
            if (whileLoop != null)
            {
                HandleWhile(context, whileLoop);
                return;
            }

            var foreachLoop = context.Node as ForEachStatementSyntax;
            if (foreachLoop != null)
            {
                HandleForeach(context, foreachLoop);
                return;
            }

            var doLoop = context.Node as DoStatementSyntax;
            if (doLoop != null)
            {
                HandleDo(context, doLoop);
            }
        }

        private void HandleFor(SyntaxNodeAnalysisContext context, ForStatementSyntax loop)
        {
            if (loop.Statement is BlockSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, loop.ForKeyword.GetLocation()));
        }

        private void HandleWhile(SyntaxNodeAnalysisContext context, WhileStatementSyntax loop)
        {
            if (loop.Statement is BlockSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, loop.WhileKeyword.GetLocation()));
        }

        private void HandleForeach(SyntaxNodeAnalysisContext context, ForEachStatementSyntax loop)
        {
            if (loop.Statement is BlockSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, loop.ForEachKeyword.GetLocation()));
        }

        private void HandleDo(SyntaxNodeAnalysisContext context, DoStatementSyntax loop)
        {
            if (loop.Statement is BlockSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, loop.DoKeyword.GetLocation()));
        }
    }
}