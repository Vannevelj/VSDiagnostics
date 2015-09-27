using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.XMLDocComments.RedundantXMLDocReturn
{
    [ExportCodeFixProvider(nameof(RedundantXmlDocReturnCodeFix), LanguageNames.CSharp), Shared]
    public class RedundantXmlDocReturnCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RedundantXmlDocReturnAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var method =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            var docComment =
                method
                    .GetLeadingTrivia()
                    .Where(n => n.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    .Select(t => t.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .First();

            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.RedundantXmlDocReturnCodeFixTitle, x => RemoveXmlReturn(context.Document, root, docComment), nameof(RedundantXmlDocReturnAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveXmlReturn(Document document, SyntaxNode root, DocumentationCommentTriviaSyntax docComment)
        {
            DocumentationCommentTriviaSyntax newDocComment = null;

            var docCommentNodes = docComment.Content;

            for (var i = 1; i < docCommentNodes.Count; i++)
            {
                var node = docCommentNodes[i] as XmlElementSyntax;

                if (node == null || node.StartTag.Name.LocalName.Text != "returns")
                {
                    continue;
                }

                newDocComment = docComment.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

                var leadingDocCommentLines = newDocComment.Content[i - 1] as XmlTextSyntax; // have to remove from the new version...

                if (leadingDocCommentLines == null)
                {
                    break;
                }

                newDocComment = newDocComment.RemoveNode(leadingDocCommentLines, SyntaxRemoveOptions.KeepNoTrivia);
            }

            var newRoot = root.ReplaceNode(docComment, newDocComment);

            return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
        }
    }
}