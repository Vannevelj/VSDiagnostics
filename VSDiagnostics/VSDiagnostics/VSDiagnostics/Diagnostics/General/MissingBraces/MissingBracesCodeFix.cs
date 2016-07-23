using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.MissingBraces
{
    [ExportCodeFixProvider(DiagnosticId.MissingBraces + "CF", LanguageNames.CSharp), Shared]
    public class MissingBracesCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(MissingBracesAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.MissingBracesCodeFixTitle,
                    x => AddBracesAsync(context, x),
                    MissingBracesAnalyzer.Rule.Id), diagnostic);
        }

        protected async Task<Document> AddBracesAsync(CodeFixContext context, CancellationToken cancellationToken)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var statement = root.FindNode(diagnosticSpan);

            var newRoot = root.ReplaceNode(statement, GetReplacementNode(statement));
            return context.Document.WithSyntaxRoot(newRoot);
        }

        private SyntaxNode GetReplacementNode(SyntaxNode statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    var ifSyntax = (IfStatementSyntax)statement;
                    return GetNewBlock(statement, ifSyntax.Statement);

                case SyntaxKind.ElseClause:
                    var elseClause = (ElseClauseSyntax)statement;
                    return GetNewBlock(statement, elseClause.Statement);

                case SyntaxKind.ForStatement:
                    var forSyntax = (ForStatementSyntax)statement;
                    return GetNewBlock(statement, forSyntax.Statement);

                case SyntaxKind.ForEachStatement:
                    var forEachSyntax = (ForEachStatementSyntax)statement;
                    return GetNewBlock(statement, forEachSyntax.Statement);

                case SyntaxKind.WhileStatement:
                    var whileSyntax = (WhileStatementSyntax)statement;
                    return GetNewBlock(statement, whileSyntax.Statement);

                case SyntaxKind.DoStatement:
                    var doSyntax = (DoStatementSyntax)statement;
                    return GetNewBlock(statement, doSyntax.Statement);

                case SyntaxKind.UsingStatement:
                    var usingSyntax = (UsingStatementSyntax)statement;
                    return GetNewBlock(statement, usingSyntax.Statement);

                case SyntaxKind.LockStatement:
                    var lockSyntax = (LockStatementSyntax)statement;
                    return GetNewBlock(statement, lockSyntax.Statement);

                case SyntaxKind.FixedStatement:
                    var fixedSyntax = (FixedStatementSyntax)statement;
                    return GetNewBlock(statement, fixedSyntax.Statement);

                case SyntaxKind.SwitchSection:
                    var switchSectionSyntax = (SwitchSectionSyntax)statement;
                    return GetNewBlock(statement, switchSectionSyntax);
            }

            return default(SyntaxNode);
        }

        private SyntaxNode GetNewBlock(SyntaxNode statement, StatementSyntax statementBody) =>
            statement.ReplaceNode(statementBody, SyntaxFactory.Block(statementBody).WithAdditionalAnnotations(Formatter.Annotation));

        private SyntaxNode GetNewBlock(SyntaxNode statement, SwitchSectionSyntax switchSection) =>
            statement.ReplaceNode(switchSection,
                SyntaxFactory.SwitchSection(switchSection.Labels,
                    SyntaxFactory.List<StatementSyntax>(new[]
                    {SyntaxFactory.Block(switchSection.Statements).WithAdditionalAnnotations(Formatter.Annotation)})));
    }
}