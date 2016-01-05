using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.RedundantXmlDocReturnCodeFixTitle,
                    x => RemoveXmlReturn(context.Document, root, docComment), nameof(RedundantXmlDocReturnAnalyzer)),
                diagnostic);
        }

        private Task<Solution> RemoveXmlReturn(Document document, SyntaxNode root, DocumentationCommentTriviaSyntax docComment)
        {
            var docCommentNodes = docComment.Content;

            var indexOfDocCommentNode = docCommentNodes.IndexOf(n =>
            {
                var node = n as XmlElementSyntax;
                return node != null && node.StartTag.Name.LocalName.Text == "returns";
            });

            var newDocComment = docComment.RemoveNode(docCommentNodes[indexOfDocCommentNode], SyntaxRemoveOptions.KeepNoTrivia);

            if (indexOfDocCommentNode == 0 || !(newDocComment.Content[indexOfDocCommentNode - 1] is XmlTextSyntax))
            {
                return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(docComment, newDocComment)).Project.Solution);
            }

            var leadingDocCommentLines = (XmlTextSyntax)newDocComment.Content[indexOfDocCommentNode - 1];
            var textTokens = leadingDocCommentLines.TextTokens;

            for (var j = textTokens.Count - 1; j >= 0; j--)
            {
                if (textTokens[j].Text.Trim() != "")
                {
                    break;
                }

                textTokens = textTokens.RemoveAt(j);
            }

            var newLeadingDocCommentLines = leadingDocCommentLines.WithTextTokens(textTokens);
            newDocComment = newDocComment.ReplaceNode(leadingDocCommentLines, newLeadingDocCommentLines);

            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(docComment, newDocComment)).Project.Solution);
        }
    }
}