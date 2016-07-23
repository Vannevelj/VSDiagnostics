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

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol,
            SyntaxKind.IfStatement,
            SyntaxKind.ElseClause,
            SyntaxKind.ForStatement,
            SyntaxKind.ForEachStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.DoStatement,
            SyntaxKind.UsingStatement,
            SyntaxKind.LockStatement,
            SyntaxKind.FixedStatement,
            SyntaxKind.SwitchSection);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                {
                    var ifStatement = (IfStatementSyntax) node;
                    if (AnalyzeIfStatement(ifStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            ifStatement.IfKeyword.GetLocation(), "n", SyntaxFacts.GetText(SyntaxKind.IfKeyword)));
                    }
                    return;
                }

                case SyntaxKind.ElseClause:
                {
                    var elseClause = (ElseClauseSyntax) node;
                    if (AnalyzeElseClause(elseClause))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            elseClause.ElseKeyword.GetLocation(), "n", SyntaxFacts.GetText(SyntaxKind.ElseKeyword)));
                    }
                    return;
                }

                case SyntaxKind.ForStatement:
                {
                    var forStatement = (ForStatementSyntax) node;
                    if (AnalyzeForStatement(forStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            forStatement.ForKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.ForKeyword)));
                    }
                    return;
                }

                case SyntaxKind.ForEachStatement:
                {
                    var forEachStatement = (ForEachStatementSyntax) node;
                    if (AnalyzeForEachStatement(forEachStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            forEachStatement.ForEachKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.ForEachKeyword)));
                    }
                    return;
                }

                case SyntaxKind.WhileStatement:
                {
                    var whileStatement = (WhileStatementSyntax) node;
                    if (AnalyzeWhileStatement(whileStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            whileStatement.WhileKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.WhileKeyword)));
                    }
                    return;
                }

                case SyntaxKind.DoStatement:
                {
                    var doStatement = (DoStatementSyntax) node;
                    if (AnalyzeDoStatement(doStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            doStatement.DoKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.DoKeyword)));
                    }
                    return;
                }

                case SyntaxKind.UsingStatement:
                {
                    var usingStatement = (UsingStatementSyntax) context.Node;
                    if (AnalyzeUsingStatement(usingStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            usingStatement.UsingKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.UsingKeyword)));
                    }
                    return;
                }

                case SyntaxKind.LockStatement:
                {
                    var lockStatement = (LockStatementSyntax) context.Node;
                    if (AnalyzeLockStatement(lockStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            lockStatement.LockKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.LockKeyword)));
                    }
                    return;
                }

                case SyntaxKind.FixedStatement:
                {
                    var fixedStatement = (FixedStatementSyntax) context.Node;
                    if (AnalyzeFixedStatement(fixedStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            fixedStatement.FixedKeyword.GetLocation(), string.Empty, SyntaxFacts.GetText(SyntaxKind.FixedKeyword)));
                    }
                    return;
                }
                
                case SyntaxKind.SwitchSection:
                {
                    var switchSection = (SwitchSectionSyntax)context.Node;
                    if (AnalyzeSwitchSectionLabel(switchSection))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            switchSection.GetLocation(), string.Empty, "switch section"));
                    }
                    return;
                }
            }
        }

        private bool AnalyzeIfStatement(IfStatementSyntax ifStatement) =>
            !ifStatement.Statement.IsKind(SyntaxKind.Block);

        private bool AnalyzeElseClause(ElseClauseSyntax elseClause) =>
            !elseClause.Statement.IsKind(SyntaxKind.Block) &&
            !elseClause.Statement.IsKind(SyntaxKind.IfStatement);

        private bool AnalyzeForStatement(ForStatementSyntax forStatement) =>
            !forStatement.Statement.IsKind(SyntaxKind.Block);

        private bool AnalyzeForEachStatement(ForEachStatementSyntax forEachStatement) =>
            !forEachStatement.Statement.IsKind(SyntaxKind.Block);

        private bool AnalyzeWhileStatement(WhileStatementSyntax whileStatement) =>
            !whileStatement.Statement.IsKind(SyntaxKind.Block);

        private bool AnalyzeDoStatement(DoStatementSyntax doStatement) =>
            !doStatement.Statement.IsKind(SyntaxKind.Block);

        private bool AnalyzeUsingStatement(UsingStatementSyntax usingStatement) =>
            !usingStatement.Statement.IsKind(SyntaxKind.Block) &&
            !usingStatement.Statement.IsKind(SyntaxKind.UsingStatement);

        private bool AnalyzeLockStatement(LockStatementSyntax lockStatement) =>
            !lockStatement.Statement.IsKind(SyntaxKind.Block) &&
            !lockStatement.Statement.IsKind(SyntaxKind.LockStatement);

        private bool AnalyzeFixedStatement(FixedStatementSyntax fixedStatement) =>
            !fixedStatement.Statement.IsKind(SyntaxKind.Block) &&
            !fixedStatement.Statement.IsKind(SyntaxKind.FixedStatement);

        private bool AnalyzeSwitchSectionLabel(SwitchSectionSyntax switchSection) =>
            switchSection.Statements.Count != 1 || !switchSection.Statements[0].IsKind(SyntaxKind.Block);
    }
}