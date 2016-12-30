using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.UsingStatementWithoutBraces
{
    [ExportCodeFixProvider(DiagnosticId.UsingStatementWithoutBraces + "CF", LanguageNames.CSharp), Shared]
    public class UsingStatementWithoutBracesCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UsingStatementWithoutBracesAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.UsingStatementWithoutBracesCodeFixTitle,
                    x => UseBracesNotationAsync(context.Document, root, (UsingStatementSyntax)statement),
                    UsingStatementWithoutBracesAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> UseBracesNotationAsync(Document document, SyntaxNode root, UsingStatementSyntax statement)
        {
            var body = SyntaxFactory.Block(statement.Statement);

            var newRoot = root.ReplaceNode(statement, statement.WithStatement(body));
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}