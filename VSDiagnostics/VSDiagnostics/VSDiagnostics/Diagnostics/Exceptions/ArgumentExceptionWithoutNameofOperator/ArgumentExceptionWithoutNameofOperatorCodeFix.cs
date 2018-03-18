using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithoutNameofOperator
{
    [ExportCodeFixProvider(DiagnosticId.ArgumentExceptionWithoutNameofOperator + "CF", LanguageNames.CSharp), Shared]
    public class ArgumentExceptionWithoutNameofOperatorCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var objectCreationExpression =
                root.FindToken(diagnosticSpan.Start)
                    .Parent.AncestorsAndSelf()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.ArgumentExceptionWithoutNameofOperatorCodeFixTitle,
                    x => UseNameofAsync(context.Document, root, objectCreationExpression),
                    ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> UseNameofAsync(Document document, SyntaxNode root, ObjectCreationExpressionSyntax objectCreationExpression)
        {
            var method = objectCreationExpression.Ancestors().OfType<MethodDeclarationSyntax>(SyntaxKind.MethodDeclaration).FirstOrDefault();
            PropertyDeclarationSyntax property = default(PropertyDeclarationSyntax);
            if (method == null)
            {
                // Fired from a property setter
                property = objectCreationExpression.Ancestors()
                        .OfType<PropertyDeclarationSyntax>(SyntaxKind.PropertyDeclaration)
                        .First();
            }
            
            var expressionArguments =
                objectCreationExpression.ArgumentList.Arguments.Select(x => x.Expression)
                                        .OfType<LiteralExpressionSyntax>();

            foreach (var expressionArgument in expressionArguments)
            {
                if (property != default(PropertyDeclarationSyntax))
                {
                    if (string.Equals(expressionArgument.Token.ValueText, "value", StringComparison.OrdinalIgnoreCase))
                    {
                        return CreateNewExpressionAsync(root, "value", objectCreationExpression, expressionArgument, document);
                    }
                }
                else
                {
                    Debug.Assert(method != null);
                    var methodParameters = method.ParameterList.Parameters;
                    foreach (var methodParameter in methodParameters)
                    {
                        if (string.Equals(methodParameter.Identifier.ValueText, expressionArgument.Token.ValueText, StringComparison.OrdinalIgnoreCase))
                        {
                            return CreateNewExpressionAsync(root, methodParameter.Identifier.ValueText, objectCreationExpression, expressionArgument, document);
                        }
                    }
                }
                
            }

            throw new InvalidOperationException("No corresponding parameter could be found");
        }

        private Task<Solution> CreateNewExpressionAsync(
            SyntaxNode root, 
            string newIdentifier,
            ObjectCreationExpressionSyntax objectCreationExpression,
            ExpressionSyntax argumentExpression,
            Document document)
        {
            var newExpression = SyntaxFactory.ParseExpression($"nameof({newIdentifier})");
            var newParent = objectCreationExpression.ReplaceNode(argumentExpression, newExpression);
            var newRoot = root.ReplaceNode(objectCreationExpression, newParent);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}