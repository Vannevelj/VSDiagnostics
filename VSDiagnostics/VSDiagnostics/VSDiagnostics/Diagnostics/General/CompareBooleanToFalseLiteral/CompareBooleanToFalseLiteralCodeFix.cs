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

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToFalseLiteral
{
    [ExportCodeFixProvider(nameof(CompareBooleanToFalseLiteralCodeFix), LanguageNames.CSharp), Shared]
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

                // I know of no cases in which this should fail, but just in case...
                if (!MapOperatorToReverseOperator.ContainsKey(binaryExpression.OperatorToken.Kind()))
                {
                    return Task.FromResult(document.Project.Solution);
                }

                var newOperator = binaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken)
                    ? MapOperatorToReverseOperator.First(kvp => kvp.Key == internalBinaryExpression.OperatorToken.Kind()).Value
                    : internalBinaryExpression.OperatorToken.Kind();

                newExpression = internalBinaryExpression.WithOperatorToken(SyntaxFactory.Token(newOperator));
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