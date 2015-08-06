﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Composition;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.SingleEmptyConstructor
{
    [ExportCodeFixProvider("SingleEmptyConstructorCodeFix", LanguageNames.CSharp), Shared]
    class SingleEmptyConstructorCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SingleEmptyConstructorAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Simplify expression", x => RemoveConstructorAsync(context.Document, root, statement), nameof(SingleEmptyConstructorAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveConstructorAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var ctorDeclarationExpression = (ConstructorDeclarationSyntax) statement;
            var newRoot = root.RemoveNode(ctorDeclarationExpression, SyntaxRemoveOptions.KeepNoTrivia);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}
