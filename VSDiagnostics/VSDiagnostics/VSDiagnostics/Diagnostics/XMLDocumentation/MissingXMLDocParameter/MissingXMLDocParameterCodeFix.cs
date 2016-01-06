using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.XMLDocumentation.MissingXMLDocParameter
{
    [ExportCodeFixProvider(nameof(MissingXmlDocParameterCodeFix), LanguageNames.CSharp), Shared]
    public class MissingXmlDocParameterCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MissingXmlDocParameterAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var method = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(diagnosticSpan.Start, diagnosticSpan.Length)) as MethodDeclarationSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.MissingXmlDocParameterCodeFixTitle,
                    x => RemoveXmlParameterNode(context.Document, root, method),
                    nameof(MissingXmlDocParameterAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveXmlParameterNode(Document document, SyntaxNode root, MethodDeclarationSyntax method)
        {
            var docComment = method.GetLeadingTrivia()
                               .Where(n => n.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                               .Select(t => t.GetStructure())
                               .OfType<DocumentationCommentTriviaSyntax>()
                               .First();

            var summaryBlock = docComment.Content.OfType<XmlElementSyntax>()
                    .First(node => node.StartTag.Name.LocalName.Text == "summary");

            var paramNames = method.ParameterList.Parameters.Select(param => param.Identifier.Text).ToList();
            var xmlParamNodes = docComment.Content.OfType<XmlElementSyntax>()
                                        .Where(element => element.StartTag.Name.LocalName.Text == "param")
                                        .ToList();

            var xmlParamNodeNames = xmlParamNodes.SelectMany(node =>
                    node.StartTag.Attributes.OfType<XmlNameAttributeSyntax>()
                        .Where(a => a.Name.LocalName.Text == "name")
                        .Select(t => t.Identifier.Identifier.Text)).ToList();

            var missingNodeParamNames = paramNames.Where(name => !xmlParamNodeNames.Contains(name));
            
            foreach (var missingNodeParamName in missingNodeParamNames)
            {
                var attribute = SyntaxFactory.XmlNameAttribute(SyntaxFactory.XmlName("name"),
                    SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken), missingNodeParamName,
                    SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))
                    .WithLeadingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));

                var paramNode = SyntaxFactory.XmlElement(
                    SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("param"),
                        SyntaxFactory.List<XmlAttributeSyntax>()
                            .Add(attribute)), SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("param")))
                    .WithLeadingTrivia(summaryBlock.GetLeadingTrivia());

                xmlParamNodes.Insert(paramNames.IndexOf(missingNodeParamName), paramNode);
            }

            var newDocComment = SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia, GetNodes(docComment, xmlParamNodes, summaryBlock));
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(docComment, newDocComment)).Project.Solution);
        }

        private SyntaxList<SyntaxNode> GetNodes(DocumentationCommentTriviaSyntax docComment, List<XmlElementSyntax> xmlParamNodes, XmlElementSyntax summaryBlock)
        {
            var nodes = SyntaxFactory.List<SyntaxNode>().Add(docComment.Content.First());

            var newLineToken = SyntaxFactory.Token(default(SyntaxTriviaList),
                SyntaxKind.XmlTextLiteralNewLineToken,
                Environment.NewLine, Environment.NewLine, default(SyntaxTriviaList));

            var docCommentToken = SyntaxFactory.Token(SyntaxFactory.TriviaList(
                SyntaxFactory.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, "///")),
                SyntaxKind.XmlTextLiteralToken, " ", " ", default(SyntaxTriviaList));

            var xmlTextElement = SyntaxFactory.XmlText(SyntaxFactory.TokenList(newLineToken, docCommentToken));

            var paramListInserted = false;
            // We already added the first one.
            for (var i = 1; i < docComment.Content.Count; i++)
            {
                var syntax = docComment.Content[i] as XmlTextSyntax;
                
                // Skip XmlTextSyntax nodes - we will re-add them
                if (syntax != null &&
                    syntax.TextTokens.All(t => t.Text.Trim() == string.Empty))
                {
                    continue;
                }

                if (!xmlParamNodes.Contains(docComment.Content[i]))
                {
                    nodes = i == 1 || docComment.Content[i] is XmlTextSyntax
                        ? nodes.Add(docComment.Content[i])
                        : nodes.AddRange(new SyntaxNode[] {xmlTextElement, docComment.Content[i]});
                }

                if (docComment.Content[i] != summaryBlock && (i != docComment.Content.Count - 1 || paramListInserted))
                {
                    continue;
                }

                nodes = xmlParamNodes.Aggregate(nodes, (current, paramNode) => current.AddRange(new SyntaxNode[] { xmlTextElement, paramNode }));
                paramListInserted = true;
            }

            var lastXmlTextSyntax = nodes.Last() as XmlTextSyntax;
            if (lastXmlTextSyntax == null || !lastXmlTextSyntax.TextTokens.Last().IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
            {
                var endNode = SyntaxFactory.XmlText(
                        SyntaxFactory.TokenList(SyntaxFactory.Token(default(SyntaxTriviaList), SyntaxKind.XmlTextLiteralNewLineToken,
                            Environment.NewLine, Environment.NewLine, default(SyntaxTriviaList))));

                return nodes.Add(endNode);
            }

            return nodes;
        }
    }
}