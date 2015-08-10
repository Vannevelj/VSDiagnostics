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
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.CompareBooleanToFalseLiteralCodeFixTitle, x => SimplifyExpressionAsync(context.Document, root, statement), nameof(CompareBooleanToFalseLiteralAnalyzer)), diagnostic);
        }

        private Task<Solution> SimplifyExpressionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var falseLiteralExpression = (LiteralExpressionSyntax) statement;
            var binaryExpression = (BinaryExpressionSyntax) falseLiteralExpression.Parent;

            ExpressionSyntax newExpression;

            if (binaryExpression.Left is BinaryExpressionSyntax || binaryExpression.Right is BinaryExpressionSyntax)
            {
                var internalBinaryExpression = binaryExpression.Left is BinaryExpressionSyntax
                    ? (BinaryExpressionSyntax) binaryExpression.Left
                    : (BinaryExpressionSyntax) binaryExpression.Right;

                var newExpressionType = internalBinaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) ^ binaryExpression.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken)
                    ? SyntaxKind.NotEqualsExpression
                    : SyntaxKind.EqualsExpression;

                newExpression = SyntaxFactory.BinaryExpression(newExpressionType, internalBinaryExpression.Left, internalBinaryExpression.Right);
            }
            else
            {
                newExpression = binaryExpression.Left == falseLiteralExpression
                    ? binaryExpression.Right
                    : binaryExpression.Left;

                if (binaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
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