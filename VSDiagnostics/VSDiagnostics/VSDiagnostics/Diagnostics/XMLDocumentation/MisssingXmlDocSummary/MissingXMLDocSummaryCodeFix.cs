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

namespace VSDiagnostics.Diagnostics.XMLDocumentation.MisssingXmlDocSummary
{
    [ExportCodeFixProvider(nameof(MissingXmlDocSummaryCodeFix), LanguageNames.CSharp), Shared]
    public class MissingXmlDocSummaryCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MissingXmlDocSummaryAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var method = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(diagnosticSpan.Start, diagnosticSpan.Length)) as MethodDeclarationSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.MisssingXmlDocSummaryCodeFixTitle,
                    x => RemoveXmlParameterNode(context.Document, root, method),
                    nameof(MissingXmlDocSummaryAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveXmlParameterNode(Document document, SyntaxNode root, MethodDeclarationSyntax method)
        {
            var docComment = method.GetLeadingTrivia()
                .Where(n => n.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                .Select(t => t.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .First();

            var summaryBodyLine = Environment.NewLine + method.GetLeadingTrivia().First() + "/// ";

            var startTag = SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary"));
            var endTag = SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary"));

            var content = SyntaxFactory.XmlText(
                    SyntaxFactory.TokenList(SyntaxFactory.ParseTokens(summaryBodyLine + "{summaryInfo}" + summaryBodyLine)));

            var summaryNode = SyntaxFactory.XmlElement(startTag, SyntaxFactory.List<XmlNodeSyntax>(new[] {content}), endTag);

            var docCommentToken = SyntaxFactory.Token(SyntaxFactory.TriviaList(
                SyntaxFactory.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///")),
                SyntaxKind.XmlTextLiteralToken, " ", " ", default(SyntaxTriviaList));

            var xmlTextElement = SyntaxFactory.XmlText(SyntaxFactory.TokenList(docCommentToken));

            var newDocComment =
                docComment.WithContent(docComment.Content.InsertRange(0,
                    new XmlNodeSyntax[]
                    {
                        xmlTextElement,
                        summaryNode.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia,
                            Environment.NewLine))
                    }));
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(docComment, newDocComment)).Project.Solution);
        }
    }
}