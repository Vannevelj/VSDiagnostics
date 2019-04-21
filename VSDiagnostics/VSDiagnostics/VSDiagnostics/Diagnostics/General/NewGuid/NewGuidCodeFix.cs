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
using Microsoft.CodeAnalysis.Simplification;
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
            // We're not adding the simplifier in this route because it doesn't seem to work
            // It works when putting it on the root node but that has too many consequences for other code
            // We do some simple introspection to see if it is fully qualified already or not
            var newExpression = "Guid.NewGuid()";
            if (statement.ChildNodes().First().IsKind(SyntaxKind.QualifiedName))
            {
                newExpression = "System.Guid.NewGuid()";
            }

            var newRoot = root.ReplaceNode(statement, SyntaxFactory.ParseExpression(newExpression));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        private Task<Document> UseEmptyGuid(Document document, SyntaxNode root, ObjectCreationExpressionSyntax statement)
        {
            var newRoot = root.ReplaceNode(statement, SyntaxFactory.ParseExpression("System.Guid.Empty").WithAdditionalAnnotations(Simplifier.Annotation));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
