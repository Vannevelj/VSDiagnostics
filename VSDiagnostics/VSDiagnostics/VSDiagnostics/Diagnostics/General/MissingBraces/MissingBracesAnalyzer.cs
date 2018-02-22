using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.MissingBraces
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingBracesAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.MissingBracesAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.MissingBracesAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.MissingBraces, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfSymbol, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeElseSymbol, SyntaxKind.ElseClause);
            context.RegisterSyntaxNodeAction(AnalyzeForSymbol, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachSymbol, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeWhileSymbol, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDoSymbol, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUsingSymbol, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(AnalyzeLockSymbol, SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(AnalyzeFixedSymbol, SyntaxKind.FixedStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchSection, SyntaxKind.SwitchSection);
        }

        private void AnalyzeIfSymbol(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            if (!ifStatement.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    ifStatement.IfKeyword.GetLocation(), "An", SyntaxFacts.GetText(SyntaxKind.IfKeyword)));
            }
        }

        private void AnalyzeElseSymbol(SyntaxNodeAnalysisContext context)
        {
            var elseClause = (ElseClauseSyntax)context.Node;
            if (!elseClause.Statement.IsKind(SyntaxKind.Block) &&
                !elseClause.Statement.IsKind(SyntaxKind.IfStatement))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    elseClause.ElseKeyword.GetLocation(), "An", SyntaxFacts.GetText(SyntaxKind.ElseKeyword)));
            }
        }

        private void AnalyzeForSymbol(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;
            if (!forStatement.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    forStatement.ForKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.ForKeyword)));
            }
        }

        private void AnalyzeForEachSymbol(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (ForEachStatementSyntax)context.Node;
            if (!forEachStatement.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    forEachStatement.ForEachKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.ForEachKeyword)));
            }
        }

        private void AnalyzeWhileSymbol(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;
            if (!whileStatement.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    whileStatement.WhileKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.WhileKeyword)));
            }
        }

        private void AnalyzeDoSymbol(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;
            if (!doStatement.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    doStatement.DoKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.DoKeyword)));
            }
        }

        private void AnalyzeUsingSymbol(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;
            if (!usingStatement.Statement.IsKind(SyntaxKind.Block) &&
                !usingStatement.Statement.IsKind(SyntaxKind.UsingStatement))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    usingStatement.UsingKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.UsingKeyword)));
            }
        }

        private void AnalyzeLockSymbol(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;
            if (!lockStatement.Statement.IsKind(SyntaxKind.Block) &&
                !lockStatement.Statement.IsKind(SyntaxKind.LockStatement))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    lockStatement.LockKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.LockKeyword)));
            }
        }

        private void AnalyzeFixedSymbol(SyntaxNodeAnalysisContext context)
        {
            var fixedStatement = (FixedStatementSyntax)context.Node;
            if (!fixedStatement.Statement.IsKind(SyntaxKind.Block) &&
                !fixedStatement.Statement.IsKind(SyntaxKind.FixedStatement))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    fixedStatement.FixedKeyword.GetLocation(), "A", SyntaxFacts.GetText(SyntaxKind.FixedKeyword)));
            }
        }

        private void AnalyzeSwitchSection(SyntaxNodeAnalysisContext context)
        {
            var switchSection = (SwitchSectionSyntax)context.Node;
            if (switchSection.Statements.Count != 1 || !switchSection.Statements[0].IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    switchSection.GetLocation(), "A", "switch section"));
            }
        }
    }
}