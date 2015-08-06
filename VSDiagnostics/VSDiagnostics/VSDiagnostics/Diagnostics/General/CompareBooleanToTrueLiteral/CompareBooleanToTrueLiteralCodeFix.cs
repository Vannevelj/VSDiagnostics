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

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral
{
    [ExportCodeFixProvider("CompareBooleanToTrueLiteral", LanguageNames.CSharp), Shared]
    public class CompareBooleanToTrueLiteralCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CompareBooleanToTrueLiteralAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Simplify expression", x => SimplifyExpressionAsync(context.Document, root, statement), nameof(CompareBooleanToTrueLiteralAnalyzer)), diagnostic);
        }

        private Task<Solution> SimplifyExpressionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var trueLiteralExpression = (LiteralExpressionSyntax) statement;
            var binaryExpression = (BinaryExpressionSyntax) trueLiteralExpression.Parent;

            ExpressionSyntax newExpression;

            if (binaryExpression.Left is BinaryExpressionSyntax || binaryExpression.Right is BinaryExpressionSyntax)
            {
                var internalBinaryExpression = binaryExpression.Left is BinaryExpressionSyntax
                    ? (BinaryExpressionSyntax)binaryExpression.Left
                    : (BinaryExpressionSyntax)binaryExpression.Right;

                var newExpressionType = internalBinaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) ^ binaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken)
                    ? SyntaxKind.NotEqualsExpression
                    : SyntaxKind.EqualsExpression;

                newExpression = SyntaxFactory.BinaryExpression(newExpressionType, internalBinaryExpression.Left, internalBinaryExpression.Right);
            }
            else
            {
                newExpression = binaryExpression.Left == trueLiteralExpression
                    ? binaryExpression.Right
                    : binaryExpression.Left;

                if (binaryExpression.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                {
                    newExpression = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression,
                        newExpression);
                }
            }

            var newRoot = root.ReplaceNode(binaryExpression, newExpression).WithAdditionalAnnotations(Formatter.Annotation);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}