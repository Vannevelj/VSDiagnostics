using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace VSDiagnostics.Diagnostics.General.TypeToVar
{
    [ExportCodeFixProvider("TypeToVar", LanguageNames.CSharp), Shared]
    public class TypeToVarCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(TypeToVarAnalyzer.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use var", x => UseVarAsync(context.Document, root, statement)), diagnostic);
        }

        public Task<Solution> UseVarAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var varIdentifier = SyntaxFactory.IdentifierName("var")
                                             .WithLeadingTrivia(statement.GetLeadingTrivia())
                                             .WithTrailingTrivia(statement.GetTrailingTrivia());

            var newRoot = root.ReplaceNode(statement, varIdentifier);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}