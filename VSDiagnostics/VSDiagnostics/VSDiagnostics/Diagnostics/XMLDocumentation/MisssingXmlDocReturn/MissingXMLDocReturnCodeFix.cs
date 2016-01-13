using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.XMLDocumentation.MisssingXmlDocReturn
{
    [ExportCodeFixProvider(nameof(MissingXmlDocReturnCodeFix), LanguageNames.CSharp), Shared]
    public class MissingXmlDocReturnCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MissingXmlDocReturnAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var method = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(diagnosticSpan.Start, diagnosticSpan.Length)) as MethodDeclarationSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.MissingXmlDocReturnCodeFixTitle,
                    x => RemoveXmlParameterNode(context.Document, root, method),
                    nameof(MissingXmlDocReturnAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveXmlParameterNode(Document document, SyntaxNode root, MethodDeclarationSyntax method)
        {
            var docComment = method.GetLeadingTrivia()
                .Where(n => n.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                .Select(t => t.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .First();

            var returnNode = SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("returns")),
                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("returns")));

            var docCommentToken = SyntaxFactory.Token(SyntaxFactory.TriviaList(
                SyntaxFactory.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///")),
                SyntaxKind.XmlTextLiteralToken, " ", " ", default(SyntaxTriviaList));

            var xmlTextElement = SyntaxFactory.XmlText(SyntaxFactory.TokenList(docCommentToken));

            var newDocComment = docComment.WithContent(docComment.Content.AddRange(new XmlNodeSyntax[] {xmlTextElement, returnNode.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, Environment.NewLine))}));
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(docComment, newDocComment)).Project.Solution);
        }
    }
}