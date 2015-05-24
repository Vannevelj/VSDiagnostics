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
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IfStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = context.Node as IfStatementSyntax;
            if (ifStatement == null)
            {
                return;
            }

            var hasElseStatement = ifStatement.Else != null;

            if (ifStatement.Statement is BlockSyntax && !hasElseStatement)
            {
                return; // If is correct and there's no else
            }

            if (ifStatement.Statement is BlockSyntax && ifStatement.Else.Statement is BlockSyntax)
            {
                return; // Both are a block
            }

            if (ifStatement.Statement is BlockSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatement.Else.ElseKeyword.GetLocation()));
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatement.IfKeyword.GetLocation()));
        }
    }
}