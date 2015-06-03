using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator
{
    [ExportCodeFixProvider("OnPropertyChangedWithoutNameOfOperator", LanguageNames.CSharp), Shared]
    public class OnPropertyChangedWithoutNameOfOperatorCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(OnPropertyChangedWithoutNameOfOperatorAnalyzer.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var argumentDeclaration = root.FindNode(diagnosticSpan).AncestorsAndSelf().OfType<ArgumentSyntax>().FirstOrDefault();
            context.RegisterCodeFix(CodeAction.Create("Use nameof", x => UseNameOfAsync(context.Document, root, argumentDeclaration)), diagnostic);
        }

        private Task<Solution> UseNameOfAsync(Document document, SyntaxNode root, ArgumentSyntax argumentDeclaration)
        {
            var properties = argumentDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First().ChildNodes().OfType<PropertyDeclarationSyntax>();
            foreach (var property in properties)
            {
                if (string.Equals(property.Identifier.ValueText, ((LiteralExpressionSyntax) argumentDeclaration.Expression).Token.ValueText, StringComparison.OrdinalIgnoreCase))
                {
                    root = root.ReplaceNode(argumentDeclaration.Expression, SyntaxFactory.ParseExpression($"nameof({property.Identifier.ValueText})"));
                    var newDocument = document.WithSyntaxRoot(root);
                    return Task.FromResult(newDocument.Project.Solution);
                }
            }

            return null;
        }
    }
}