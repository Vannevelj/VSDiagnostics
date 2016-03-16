﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.Attributes.AttributeWithEmptyArgumentList
{
    [ExportCodeFixProvider(nameof(AttributeWithEmptyArgumentListCodeFix), LanguageNames.CSharp), Shared]
    public class AttributeWithEmptyArgumentListCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AttributeWithEmptyArgumentListAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.AttributeWithEmptyArgumentListCodeFixTitle,
                    x => RemoveEmptyArgumentListAsync(context.Document, root, statement),
                    AttributeWithEmptyArgumentListAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> RemoveEmptyArgumentListAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var newRoot = root.RemoveNode(((AttributeSyntax)statement).ArgumentList, SyntaxRemoveOptions.KeepNoTrivia);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}