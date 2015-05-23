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
using Microsoft.CodeAnalysis.Editing;

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

            context.RegisterCodeFix(CodeAction.Create("Use nameof", x => UseNameofAsync(context.Document, objectCreationExpression, context.CancellationToken)), diagnostic);
        }

        private async Task<Document> UseNameofAsync(Document document, ObjectCreationExpressionSyntax objectCreationExpression, CancellationToken cancellationToken)
        {
            var syntaxGenerator = SyntaxGenerator.GetGenerator(document);
            var method = objectCreationExpression.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var methodSymbol = semanticModel.GetSymbolInfo(method).Symbol as IMethodSymbol;
            var methodParameters = method.ParameterList.Parameters;
            var expressionArguments = objectCreationExpression.ArgumentList.Arguments;

            foreach (var expressionArgument in expressionArguments)
            {
                foreach (var methodParameter in methodParameters)
                {
                    if (string.Equals((string) methodParameter.Identifier.Value, (string) ((LiteralExpressionSyntax) expressionArgument.Expression).Token.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        //var newArgument = syntaxGenerator.WithExpression(expressionArgument, syntaxGenerator.InvocationExpression(SyntaxFactory.InvocationExpression(SyntaxFactory.ExpressionStatement()))
                        //var newMethod = syntaxGenerator.ObjectCreationExpression(objectCreationExpression.WithArgumentList())
                    }
                }
            }
            return null;
        }
    }
}