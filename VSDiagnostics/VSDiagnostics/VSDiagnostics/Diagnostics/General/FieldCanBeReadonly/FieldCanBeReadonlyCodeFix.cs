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

namespace VSDiagnostics.Diagnostics.General.FieldCanBeReadonly
{
    [ExportCodeFixProvider(DiagnosticId.FieldCanBeReadonly + "CF", LanguageNames.CSharp), Shared]
    public class FieldCanBeReadonlyCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FieldCanBeReadonlyAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var fieldDeclaration = (FieldDeclarationSyntax)root.FindNode(diagnosticSpan).Parent.Parent;

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.FieldCanBeReadonlyCodeFixTitle,
                    x => MakeReadonlyAsync(context.Document, root, fieldDeclaration), FieldCanBeReadonlyAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Document> MakeReadonlyAsync(Document document, SyntaxNode root, FieldDeclarationSyntax declaration)
        {
            var newDeclaration = declaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

            var newRoot = root.ReplaceNode(declaration, newDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument);
        }
    }
}