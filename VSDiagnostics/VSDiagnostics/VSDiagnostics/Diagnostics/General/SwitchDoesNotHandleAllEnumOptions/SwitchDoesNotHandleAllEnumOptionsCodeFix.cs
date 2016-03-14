using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSDiagnostics.Diagnostics.General.SingleEmptyConstructor;

namespace VSDiagnostics.Diagnostics.General.SwitchDoesNotHandleAllEnumOptions
{
    [ExportCodeFixProvider(nameof(SingleEmptyConstructorCodeFix), LanguageNames.CSharp), Shared]
    internal class SwitchDoesNotHandleAllEnumOptionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(SingleEmptyConstructorAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.SwitchDoesNotHandleAllEnumOptionsCodeFixTitle,
                    x => AddMissingCaseAsync(context.Document, root, statement),
                    SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> AddMissingCaseAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            /*var constructorDeclaration = (ConstructorDeclarationSyntax) statement;
            var newRoot = root.RemoveNode(constructorDeclaration, SyntaxRemoveOptions.KeepNoTrivia);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);*/
            return null;
        }
    }
}