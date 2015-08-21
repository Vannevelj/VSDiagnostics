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
using Microsoft.CodeAnalysis.Rename;

namespace VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix
{
    [ExportCodeFixProvider(nameof(AsyncMethodWithoutAsyncSuffixCodeFix), LanguageNames.CSharp), Shared]
    public class AsyncMethodWithoutAsyncSuffixCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.AsyncMethodWithoutAsyncSuffixCodeFixTitle, x => AddSuffixAsync(context.Document, methodDeclaration, context.CancellationToken), nameof(AsyncMethodWithoutAsyncSuffixAnalyzer)),
                diagnostic);
        }

        private async Task<Solution> AddSuffixAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var methodSymbol = (await document.GetSemanticModelAsync(cancellationToken)).GetDeclaredSymbol(methodDeclaration);
            return await Renamer.RenameSymbolAsync(
                document.Project.Solution,
                methodSymbol,
                methodDeclaration.Identifier.Text + "Async",
                document.Project.Solution.Workspace.Options,
                cancellationToken);
        }
    }
}