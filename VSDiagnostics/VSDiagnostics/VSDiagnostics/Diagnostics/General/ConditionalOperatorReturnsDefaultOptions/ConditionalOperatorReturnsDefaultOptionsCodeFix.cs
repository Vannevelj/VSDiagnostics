using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsDefaultOptions
{
    [ExportCodeFixProvider("ConditionalOperatorReturnsDefaultOptions", LanguageNames.CSharp), Shared]
    public class ConditionalOperatorReturnsDefaultOptionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConditionalOperatorReturnsDefaultOptionsAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Remove conditional", x => RemoveConditionalAsync(context.Document, root, statement)), diagnostic);
        }

        private Task<Solution> RemoveConditionalAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            throw new NotImplementedException();
        }
    }
}