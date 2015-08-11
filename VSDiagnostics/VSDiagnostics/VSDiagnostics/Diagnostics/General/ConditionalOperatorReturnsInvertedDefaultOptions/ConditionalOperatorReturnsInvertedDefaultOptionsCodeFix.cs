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

namespace VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsInvertedDefaultOptions
{
    [ExportCodeFixProvider("ConditionalOperatorReturnsInvertedDefaultOptions", LanguageNames.CSharp), Shared]
    public class ConditionalOperatorReturnsInvertedDefaultOptionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.ConditionalOperatorReturnsInvertedDefaultOptionsCodeFixTitle, x => RemoveConditionalAsync(context.Document, root, statement), nameof(ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveConditionalAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var conditionalExpression = (ConditionalExpressionSyntax) statement;
            var newExpression = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, conditionalExpression.Condition);

            if (!(conditionalExpression.Condition is ParenthesizedExpressionSyntax) &&
                !(conditionalExpression.Condition is IdentifierNameSyntax))
            {
                newExpression = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression,
                    SyntaxFactory.ParenthesizedExpression(conditionalExpression.Condition));
            }

            var newRoot = root.ReplaceNode(conditionalExpression, newExpression).WithAdditionalAnnotations(Formatter.Annotation);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}