using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithNameofOperator
{
    [ExportCodeFixProvider("ArgumentExceptionWithNameofOperator", LanguageNames.CSharp), Shared]
    public class ArgumentExceptionWithNameofOperatorCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ArgumentExceptionWithNameofOperatorAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var objectCreationExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().First();

            context.RegisterCodeFix(CodeAction.Create("Use nameof", x => UseNameofAsync(context.Document, root, objectCreationExpression, context.CancellationToken)), diagnostic);
        }

        private Task<Solution> UseNameofAsync(Document document, SyntaxNode root, ObjectCreationExpressionSyntax objectCreationExpression, CancellationToken cancellationToken)
        {
            var method = objectCreationExpression.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var methodParameters = method.ParameterList.Parameters;
            var expressionArguments = objectCreationExpression.ArgumentList.Arguments;

            foreach (var expressionArgument in expressionArguments)
            {
                foreach (var methodParameter in methodParameters)
                {
                    if (string.Equals((string) methodParameter.Identifier.Value, (string) ((LiteralExpressionSyntax) expressionArgument.Expression).Token.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        var newExpression = SyntaxFactory.ParseExpression($"nameof({methodParameter.Identifier})");
                        var newParent = objectCreationExpression.ReplaceNode(expressionArgument.Expression, newExpression);
                        var newRoot = root.ReplaceNode(objectCreationExpression, newParent);
                        var newDocument = document.WithSyntaxRoot(newRoot);
                        return Task.FromResult(newDocument.Project.Solution);
                    }
                }
            }
            return null;
        }
    }
}