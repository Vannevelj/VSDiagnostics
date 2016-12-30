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
            if (context.Node.IsKind(SyntaxKind.ForStatement))
            {
                HandleFor(context, (ForStatementSyntax)context.Node);
                return;
            }
            
            if (context.Node.IsKind(SyntaxKind.WhileStatement))
            {
                HandleWhile(context, (WhileStatementSyntax)context.Node);
                return;
            }
            
            if (context.Node.IsKind(SyntaxKind.ForEachStatement))
            {
                HandleForeach(context, (ForEachStatementSyntax)context.Node);
                return;
            }
            
            if (context.Node.IsKind(SyntaxKind.DoStatement))
            {
                HandleDo(context, (DoStatementSyntax)context.Node);
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