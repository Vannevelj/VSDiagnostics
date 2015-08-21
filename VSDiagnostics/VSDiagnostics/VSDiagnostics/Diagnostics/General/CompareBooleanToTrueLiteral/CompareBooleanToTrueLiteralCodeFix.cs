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
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral
{
    [ExportCodeFixProvider(nameof(CompareBooleanToTrueLiteralCodeFix), LanguageNames.CSharp), Shared]
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
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.CompareBooleanToTrueLiteralCodeFixTitle, x => SimplifyExpressionAsync(context.Document, root, statement), nameof(CompareBooleanToTrueLiteralAnalyzer)), diagnostic);
        }

        private Task<Solution> SimplifyExpressionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var trueLiteralExpression = (LiteralExpressionSyntax) statement;
            var binaryExpression = (BinaryExpressionSyntax) trueLiteralExpression.Parent;

            ExpressionSyntax newExpression;

            if (binaryExpression.Left is BinaryExpressionSyntax || binaryExpression.Right is BinaryExpressionSyntax)
            {
                var internalBinaryExpression = binaryExpression.Left is BinaryExpressionSyntax
                    ? (BinaryExpressionSyntax) binaryExpression.Left
                    : (BinaryExpressionSyntax) binaryExpression.Right;

                // I know of no cases in which this should fail, but just in case...
                if (!MapOperatorToReverseOperator.ContainsKey(binaryExpression.OperatorToken.Kind()))
                {
                    return Task.FromResult(document.Project.Solution);
                }

                var newOperator = binaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken)
                    ? internalBinaryExpression.OperatorToken.Kind()
                    : MapOperatorToReverseOperator.First(kvp => kvp.Key == internalBinaryExpression.OperatorToken.Kind()).Value;

                newExpression = internalBinaryExpression.WithOperatorToken(SyntaxFactory.Token(newOperator));
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

        private static readonly Dictionary<SyntaxKind, SyntaxKind> MapOperatorToReverseOperator =
            new Dictionary<SyntaxKind, SyntaxKind>
            {
                        {SyntaxKind.EqualsEqualsToken, SyntaxKind.ExclamationEqualsToken},
                        {SyntaxKind.ExclamationEqualsToken, SyntaxKind.EqualsEqualsToken},
                        {SyntaxKind.GreaterThanEqualsToken, SyntaxKind.LessThanToken},
                        {SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken},
                        {SyntaxKind.LessThanEqualsToken, SyntaxKind.GreaterThanToken},
                        {SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken},
            };
    }
}