using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.LoopStatementWithoutBraces
{
    [ExportCodeFixProvider("LoopWithoutBraces", LanguageNames.CSharp), Shared]
    public class LoopStatementWithoutBracesCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(LoopStatementWithoutBracesAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.LoopStatementWithoutBracesCodeFixTitle, x => UseBracesNotationAsync(context.Document, root, statement), nameof(LoopStatementWithoutBracesAnalyzer)), diagnostic);
        }

        private Task<Solution> UseBracesNotationAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            SyntaxNode newBlock = null;

            var forSyntax = statement as ForStatementSyntax;
            if (forSyntax != null)
            {
                newBlock = GetNewBlock(statement, forSyntax.Statement);
            }

            var whileSyntax = statement as WhileStatementSyntax;
            if (whileSyntax != null)
            {
                newBlock = GetNewBlock(statement, whileSyntax.Statement);
            }

            var foreachSyntax = statement as ForEachStatementSyntax;
            if (foreachSyntax != null)
            {
                newBlock = GetNewBlock(statement, foreachSyntax.Statement);
            }

            var doSyntax = statement as DoStatementSyntax;
            if (doSyntax != null)
            {
                newBlock = GetNewBlock(statement, doSyntax.Statement);
            }

            var newRoot = root.ReplaceNode(statement, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }

        private SyntaxNode GetNewBlock(SyntaxNode statement, StatementSyntax statementBody)
        {
            var body = SyntaxFactory.Block(statementBody);
            return statement.ReplaceNode(statementBody, body);
        }
    }
}