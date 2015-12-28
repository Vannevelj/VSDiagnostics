﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifier
{
    [ExportCodeFixProvider(nameof(TestMethodWithoutPublicModifierCodeFix), LanguageNames.CSharp), Shared]
    public class TestMethodWithoutPublicModifierCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(TestMethodWithoutPublicModifierAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodDeclaration =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.TestMethodWithoutPublicModifierCodeFixTitle,
                    x => MakePublicAsync(context.Document, root, methodDeclaration),
                    TestMethodWithoutPublicModifierAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> MakePublicAsync(Document document, SyntaxNode root, MethodDeclarationSyntax method)
        {
            var generator = SyntaxGenerator.GetGenerator(document);
            var newMethod = generator.WithAccessibility(method, Accessibility.Public);
            var newRoot = root.ReplaceNode(method, newMethod);
            return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
        }
    }
}