using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using CSharpAttributeSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax;
using VisualBasicAttributeSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax;

namespace VSDiagnostics.Diagnostics.Attributes.AttributeWithEmptyArgumentList
{
    [ExportCodeFixProvider(nameof(AttributeWithEmptyArgumentListCodeFix), LanguageNames.CSharp,
        LanguageNames.VisualBasic), Shared]
    public class AttributeWithEmptyArgumentListCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AttributeWithEmptyArgumentListAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.AttributeWithEmptyArgumentListCodeFixTitle,
                    x => RemoveEmptyArgumentListAsync(context.Document, root, statement),
                    AttributeWithEmptyArgumentListAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> RemoveEmptyArgumentListAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            SyntaxNode newRoot = null;

            if (statement is CSharpAttributeSyntax)
            {
                newRoot = RemoveEmptyArgumentListCSharp(root, (CSharpAttributeSyntax) statement);
            }
            else if (statement is VisualBasicAttributeSyntax)
            {
                newRoot = RemoveEmptyArgumentListVisualBasic(root, (VisualBasicAttributeSyntax) statement);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }

        private SyntaxNode RemoveEmptyArgumentListCSharp(SyntaxNode root, CSharpAttributeSyntax attributeExpression)
        {
            return root.RemoveNode(attributeExpression.ArgumentList, SyntaxRemoveOptions.KeepNoTrivia);
        }

        private SyntaxNode RemoveEmptyArgumentListVisualBasic(SyntaxNode root,
            VisualBasicAttributeSyntax attributeExpression)
        {
            return root.RemoveNode(attributeExpression.ArgumentList, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}