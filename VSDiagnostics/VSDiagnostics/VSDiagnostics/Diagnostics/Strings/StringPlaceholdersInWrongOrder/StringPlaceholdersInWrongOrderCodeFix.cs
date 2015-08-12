using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.Strings.StringPlaceholdersInWrongOrder
{
    [ExportCodeFixProvider("StringPlaceHoldersInWrongOrder", LanguageNames.CSharp), Shared]
    public class StringPlaceHoldersInWrongOrderCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StringPlaceholdersInWrongOrderAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var stringFormatInvocation = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First(); // Fix so we get the invocation
            context.RegisterCodeFix(CodeAction.Create("Re-order placeholders", x => ReOrderPlaceholdersAsync(context.Document, root, stringFormatInvocation), nameof(StringPlaceholdersInWrongOrderAnalyzer)),
                diagnostic);
        }

        private static Task<Solution> ReOrderPlaceholdersAsync(Document document, SyntaxNode root, InvocationExpressionSyntax stringFormatInvocation)
        {
            throw new NotImplementedException();
        }
    }
}