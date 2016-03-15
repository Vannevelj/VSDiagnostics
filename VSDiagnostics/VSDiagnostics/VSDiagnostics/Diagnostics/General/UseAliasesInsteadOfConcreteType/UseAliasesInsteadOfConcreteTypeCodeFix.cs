using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Simplification;

namespace VSDiagnostics.Diagnostics.General.UseAliasesInsteadOfConcreteType
{
    [ExportCodeFixProvider(nameof(UseAliasesInsteadOfConcreteTypeCodeFix), LanguageNames.CSharp), Shared]
    public class UseAliasesInsteadOfConcreteTypeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeCodeFixTitle,
                    x => UseAliasAsync(context.Document, root, statement),
                    UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.Id), diagnostic);
        }

        private async Task<Solution> UseAliasAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var newRoot = root.ReplaceNode(statement, statement.WithAdditionalAnnotations(Simplifier.Annotation));
            var newDocument = await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot));

            return newDocument.Project.Solution;
        }
    }
}