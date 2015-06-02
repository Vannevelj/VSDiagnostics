using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.IfStatementWithoutBraces
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementWithoutBracesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(IfStatementWithoutBracesAnalyzer);
        internal const string Title = "If blocks should use braces to denote start and end.";
        internal const string Message = "An if-statement should be written using braces.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IfStatement, SyntaxKind.ElseClause);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = context.Node as IfStatementSyntax;
            if (ifStatement != null)
            {
                HandleIf(context, ifStatement);
            }

            var elseClause = context.Node as ElseClauseSyntax;
            if (elseClause != null)
            {
                HandleElse(context, elseClause);
            }
        }

        private void HandleIf(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.Statement is BlockSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatement.IfKeyword.GetLocation()));
        }

        private void HandleElse(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            if (elseClause.Statement is BlockSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, elseClause.ElseKeyword.GetLocation()));
        }
    }
}