using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using CompilationUnitSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax;
using CSharpSyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using VisualBasicSyntaxFactory = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute
{
    [ExportCodeFixProvider(nameof(EnumCanHaveFlagsAttributeCodeFix), LanguageNames.CSharp, LanguageNames.VisualBasic), Shared]
    public class EnumCanHaveFlagsAttributeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(EnumCanHaveFlagsAttributeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.EnumCanHaveFlagsAttributeCodeFixTitle, x => AddFlagAttributeAsync(context.Document, root, statement), nameof(EnumCanHaveFlagsAttributeAnalyzer)), diagnostic);
        }

        private Task<Solution> AddFlagAttributeAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            SyntaxNode newRoot = null;

            if (statement is EnumDeclarationSyntax)
            {
                newRoot = AddFlagAttributeCSharp(document, root, (EnumDeclarationSyntax)statement);
            }
            else if (statement is EnumStatementSyntax)
            {
                newRoot = AddFlagAttributeVisualBasic(document, root, (EnumStatementSyntax)statement);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }

        private SyntaxNode AddFlagAttributeCSharp(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var flagsAttribute = CSharpSyntaxFactory.Attribute(CSharpSyntaxFactory.ParseName("Flags"));
            var newStatement = generator.AddAttributes(statement, flagsAttribute);

            var newRoot = root.ReplaceNode(statement, newStatement);

            var compilationUnit = (CompilationUnitSyntax)newRoot;

            var usingSystemDirective = CSharpSyntaxFactory.UsingDirective(CSharpSyntaxFactory.ParseName("System"));
            var usingDirectives = compilationUnit.Usings.Select(u => u.Name.GetText().ToString());

            if (usingDirectives.All(u => u != usingSystemDirective.Name.GetText().ToString()))
            {
                var usings = compilationUnit.Usings.Add(usingSystemDirective).OrderBy(u => u.Name.GetText().ToString());

                newRoot =
                    compilationUnit.WithUsings(CSharpSyntaxFactory.List(usings))
                        .WithAdditionalAnnotations(Formatter.Annotation);
            }

            return newRoot;
        }

        private SyntaxNode AddFlagAttributeVisualBasic(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var flagsAttribute = VisualBasicSyntaxFactory.Attribute(VisualBasicSyntaxFactory.ParseName("Flags"));
            var newStatement = generator.AddAttributes(statement, flagsAttribute);

            // no need to add a `using` directive because creating a VB.NET project in VS
            // adds a project reference to the `System` namespace
            // to verify this, check the References tab of the project properties
            // (bottom of the Debug tab)
            return root.ReplaceNode(statement, newStatement);
        }
    }
}