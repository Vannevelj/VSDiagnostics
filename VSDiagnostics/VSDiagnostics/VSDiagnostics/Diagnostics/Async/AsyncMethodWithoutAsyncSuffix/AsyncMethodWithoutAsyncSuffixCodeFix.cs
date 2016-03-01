using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix
{
    [ExportCodeFixProvider(nameof(AsyncMethodWithoutAsyncSuffixCodeFix), LanguageNames.CSharp), Shared]
    public class AsyncMethodWithoutAsyncSuffixCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var methodDeclaration =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.AsyncMethodWithoutAsyncSuffixCodeFixTitle,
                    x => AddSuffixAsync(context.Document, methodDeclaration, root, context.CancellationToken),
                    AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id),
                diagnostic);
        }

        private async Task<Solution> AddSuffixAsync(Document document, MethodDeclarationSyntax methodDeclaration, SyntaxNode root,
            CancellationToken cancellationToken)
        {
            return await RenameHelper.RenameSymbolAsync(document, root, methodDeclaration.Identifier, methodDeclaration.Identifier.Text + "Async", cancellationToken);
        }
    }
}