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

namespace VSDiagnostics.Diagnostics.Async.SyncMethodWithAsyncSuffix
{
    [ExportCodeFixProvider(DiagnosticId.SyncMethodWithAsyncSuffix + "CF", LanguageNames.CSharp), Shared]
    public class SyncMethodWithAsyncSuffixCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(SyncMethodWithAsyncSuffixAnalyzer.Rule.Id);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var methodDeclaration =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.SyncMethodWithAsyncSuffixAnalyzerCodeFixTitle,
                    x => RemoveSuffixAsync(context.Document, methodDeclaration, root, x),
                    SyncMethodWithAsyncSuffixAnalyzer.Rule.Id),
                diagnostic);
        }

        private async Task<Solution> RemoveSuffixAsync(Document document, MethodDeclarationSyntax methodDeclaration, SyntaxNode root,
                                                       CancellationToken cancellationToken)
        {
            var origMethodName = methodDeclaration.Identifier.Text;
            var newMethodName = origMethodName.Substring(0, origMethodName.Length - "Async".Length);
            return await RenameHelper.RenameSymbolAsync(document, root, methodDeclaration.Identifier, newMethodName, cancellationToken);
        }
    }
}