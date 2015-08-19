using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace VSDiagnostics.Diagnostics.Tests.RemoveTestSuffix
{
    [ExportCodeFixProvider("RemoveTestSuffix", LanguageNames.CSharp), Shared]
    public class RemoveTestSuffixCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RemoveTestSuffixAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.RemoveTestSuffixCodeFixTitle, x => RemoveTestSuffix(context.Document, root, methodDeclaration), nameof(RemoveTestSuffixAnalyzer)), diagnostic);
        }

        private async Task<Solution> RemoveTestSuffix(Document document, SyntaxNode root, MethodDeclarationSyntax methodDeclaration)
        {
            var methodSymbol = (await document.GetSemanticModelAsync()).GetDeclaredSymbol(methodDeclaration);
            var newMethodName = methodDeclaration.Identifier.Text.Remove(methodDeclaration.Identifier.Text.Length - 4);

            return await Renamer.RenameSymbolAsync(
                document.Project.Solution,
                methodSymbol,
                newMethodName,
                document.Project.Solution.Workspace.Options);
        }
    }
}