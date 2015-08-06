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

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToFalseLiteral
{
    [ExportCodeFixProvider("CompareBooleanToTrueLiteral", LanguageNames.CSharp), Shared]
    public class CompareBooleanToFalseLiteralCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CompareBooleanToFalseLiteralAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Simplify expression", x => SimplifyExpressionAsync(context.Document, root, statement), nameof(CompareBooleanToFalseLiteralAnalyzer)), diagnostic);
        }

        private Task<Solution> SimplifyExpressionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var falseLiteralExpression = (LiteralExpressionSyntax) statement;
            var binaryExpression = (BinaryExpressionSyntax) falseLiteralExpression.Parent;
            SyntaxNode newRoot;

            if (binaryExpression.Left is BinaryExpressionSyntax)
            {
                var internalBinaryExpression = binaryExpression.Left as BinaryExpressionSyntax;

                var newExpression = SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, internalBinaryExpression.Left, internalBinaryExpression.Right);

                newRoot = root.ReplaceNode(binaryExpression, newExpression).WithAdditionalAnnotations(Formatter.Annotation);
            }
            else if (binaryExpression.Right is BinaryExpressionSyntax)
            {
                var internalBinaryExpression = binaryExpression.Right as BinaryExpressionSyntax;

                var newExpression = SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, internalBinaryExpression.Left, internalBinaryExpression.Right);

                newRoot = root.ReplaceNode(binaryExpression, newExpression).WithAdditionalAnnotations(Formatter.Annotation);
            }
            else
            {
                var expressionToKeep = binaryExpression.Left == falseLiteralExpression
                    ? binaryExpression.Right
                    : binaryExpression.Left;

                var newExpression = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expressionToKeep);
                newRoot = root.ReplaceNode(binaryExpression, newExpression).WithAdditionalAnnotations(Formatter.Annotation);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}