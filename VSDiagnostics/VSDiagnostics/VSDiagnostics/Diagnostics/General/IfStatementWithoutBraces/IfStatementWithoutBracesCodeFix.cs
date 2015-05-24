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
    [ExportCodeFixProvider("IfStatementWithoutBraces", LanguageNames.CSharp), Shared]
    public class IfStatementWithoutBracesCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(IfStatementWithoutBracesAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var ifStatement = (IfStatementSyntax) root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use braces", x => UseBracesNotation(context.Document, root, ifStatement)), diagnostic);
        }

        private Task<Solution> UseBracesNotation(Document document, SyntaxNode root, IfStatementSyntax ifStatement)
        {
            var body = SyntaxFactory.Block(ifStatement.Statement);

            var newBlock = ifStatement.ReplaceNode(ifStatement.Statement, body);
            var newRoot = root.ReplaceNode(ifStatement, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}