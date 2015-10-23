using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.IfStatementWithoutBraces
{
    [ExportCodeFixProvider(nameof(IfStatementWithoutBracesCodeFix), LanguageNames.CSharp), Shared]
    public class IfStatementWithoutBracesCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(IfStatementWithoutBracesAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.IfStatementWithoutBracesCodeFixTitle,
                    x => UseBracesNotationAsync(context.Document, root, statement),
                    IfStatementWithoutBracesAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> UseBracesNotationAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            SyntaxNode newBlock = null;

            var ifSyntax = statement as IfStatementSyntax;
            if (ifSyntax != null)
            {
                newBlock = GetNewBlock(statement, ifSyntax.Statement);
            }

            var elseSyntax = statement as ElseClauseSyntax;
            if (elseSyntax != null)
            {
                newBlock = GetNewBlock(statement, elseSyntax.Statement);
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