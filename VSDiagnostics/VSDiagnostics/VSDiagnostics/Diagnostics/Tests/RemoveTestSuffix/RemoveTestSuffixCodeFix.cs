using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Tests.RemoveTestSuffix
{
    [ExportCodeFixProvider(nameof(RemoveTestSuffixCodeFix), LanguageNames.CSharp), Shared]
    public class RemoveTestSuffixCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(RemoveTestSuffixAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodDeclaration =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.RemoveTestSuffixCodeFixTitle,
                    x => RemoveTestSuffixAsync(context.Document, root, methodDeclaration, context.CancellationToken), RemoveTestSuffixAnalyzer.Rule.Id),
                diagnostic);
        }

        private async Task<Solution> RemoveTestSuffixAsync(Document document, SyntaxNode root,
            MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var newMethodName = methodDeclaration.Identifier.Text.Remove(methodDeclaration.Identifier.Text.Length - 4);
            return await RenameHelper.RenameSymbolAsync(document, root, methodDeclaration.Identifier, newMethodName, cancellationToken);
        }
    }
}