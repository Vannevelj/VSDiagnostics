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
using Microsoft.CodeAnalysis.Formatting;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NewGuid
{
    [ExportCodeFixProvider(DiagnosticId.NewGuid + "CF", LanguageNames.CSharp), Shared]
    public class NewGuidCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NewGuidAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan).DescendantNodesAndSelf().OfType<ObjectCreationExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.NewGuidUseNewGuidCodeFixTitle,
                    x => UseNewGuid(context.Document, root, statement), $"{NewGuidAnalyzer.Rule.Id}A"), diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.NewGuidUseEmptyGuidCodeFixTitle,
                    x => UseEmptyGuid(context.Document, root, statement), $"{NewGuidAnalyzer.Rule.Id}B"), diagnostic);
        }

        private Task<Document> UseNewGuid(Document document, SyntaxNode root, ObjectCreationExpressionSyntax statement)
        {
            var newRoot = root.ReplaceNode(statement, SyntaxFactory.ParseExpression("Guid.NewGuid()"));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        private Task<Document> UseEmptyGuid(Document document, SyntaxNode root, ObjectCreationExpressionSyntax statement)
        {
            var newRoot = root.ReplaceNode(statement, SyntaxFactory.ParseExpression("Guid.Empty"));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
