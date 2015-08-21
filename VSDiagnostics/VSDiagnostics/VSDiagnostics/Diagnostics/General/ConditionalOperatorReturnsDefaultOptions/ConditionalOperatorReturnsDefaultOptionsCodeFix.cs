using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsDefaultOptions
{
    [ExportCodeFixProvider(nameof(ConditionalOperatorReturnsDefaultOptionsCodeFix), LanguageNames.CSharp), Shared]
    public class ConditionalOperatorReturnsDefaultOptionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.ConditionalOperatorReturnsDefaultOptionsCodeFixTitle, x => RemoveConditionalAsync(context.Document, root, statement), nameof(ConditionalOperatorReturnsDefaultOptionsAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveConditionalAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var conditionalExpression = (ConditionalExpressionSyntax) statement;

            var newRoot = root.ReplaceNode(conditionalExpression, conditionalExpression.Condition).WithAdditionalAnnotations(Formatter.Annotation);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}