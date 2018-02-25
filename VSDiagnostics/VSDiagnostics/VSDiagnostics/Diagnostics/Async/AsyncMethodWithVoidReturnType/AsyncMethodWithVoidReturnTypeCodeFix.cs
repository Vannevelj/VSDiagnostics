using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Async.AsyncMethodWithVoidReturnType
{
    [ExportCodeFixProvider(DiagnosticId.AsyncMethodWithVoidReturnType + "CF", LanguageNames.CSharp), Shared]
    public class AsyncMethodWithVoidReturnTypeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AsyncMethodWithVoidReturnTypeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var methodDeclaration =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.AsyncMethodWithVoidReturnTypeCodeFixTitle,
                    x => ChangeReturnTypeAsync(context.Document, methodDeclaration, root, x),
                    AsyncMethodWithVoidReturnTypeAnalyzer.Rule.Id),
                diagnostic);
        }

        private Task<Document> ChangeReturnTypeAsync(Document document, MethodDeclarationSyntax methodDeclaration, SyntaxNode root, CancellationToken cancellationToken)
        {
            var newMethod = methodDeclaration.WithReturnType(SyntaxFactory.ParseTypeName("Task").WithAdditionalAnnotations(Formatter.Annotation));
            var newRoot = root.ReplaceNode(methodDeclaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            
            return Task.FromResult(newDocument);
        }
    }
}
