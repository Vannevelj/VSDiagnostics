using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using VSDiagnostics.Utilities;


namespace VSDiagnostics.Diagnostics.General.RedundantPrivateSetter
{
    [ExportCodeFixProvider(DiagnosticId.RedundantPrivateSetter + "CF", LanguageNames.CSharp), Shared]
    public class RedundantPrivateSetterCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RedundantPrivateSetterAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.NullableToShorthandCodeFixTitle, x => UseReadOnlyPropertyAsync(context.Document, root, declaration), RedundantPrivateSetterAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Document> UseReadOnlyPropertyAsync(Document document, SyntaxNode root, SyntaxNode setAccessor)
        {
            root = root.RemoveNode(setAccessor, SyntaxRemoveOptions.KeepNoTrivia);
            return Task.FromResult(document.WithSyntaxRoot(root));
        }
    }
}
