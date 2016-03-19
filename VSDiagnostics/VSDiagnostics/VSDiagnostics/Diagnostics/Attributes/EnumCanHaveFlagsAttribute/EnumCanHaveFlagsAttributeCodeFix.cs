using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute
{
    [ExportCodeFixProvider(DiagnosticId.EnumCanHaveFlagsAttribute, LanguageNames.CSharp),
     Shared]
    public class EnumCanHaveFlagsAttributeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(EnumCanHaveFlagsAttributeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.EnumCanHaveFlagsAttributeCodeFixTitle,
                    x => AddFlagAttributeAsync(context.Document, root, statement),
                    EnumCanHaveFlagsAttributeAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> AddFlagAttributeAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var flagsAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Flags"));
            var newStatement = generator.AddAttributes(statement, flagsAttribute);

            var newRoot = root.ReplaceNode(statement, newStatement);

            var compilationUnit = (CompilationUnitSyntax) newRoot;

            var usingSystemDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            var usingDirectives = compilationUnit.Usings.Select(u => u.Name.GetText().ToString());

            if (usingDirectives.All(u => u != usingSystemDirective.Name.GetText().ToString()))
            {
                newRoot = generator.AddNamespaceImports(compilationUnit, usingSystemDirective);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}