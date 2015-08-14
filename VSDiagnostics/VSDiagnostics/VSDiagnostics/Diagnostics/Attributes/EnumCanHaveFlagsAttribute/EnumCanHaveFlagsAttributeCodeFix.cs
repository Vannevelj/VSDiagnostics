using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute
{
    [ExportCodeFixProvider("EnumCanHaveFlagsAttribute", LanguageNames.CSharp), Shared]
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
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.EnumCanHaveFlagsAttributeCodeFixTitle, x => AddFlagAttribute(context.Document, root, statement), nameof(EnumCanHaveFlagsAttributeAnalyzer)), diagnostic);
        }

        private async Task<Solution> AddFlagAttribute(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var enumDeclarationExpression = (EnumDeclarationSyntax) statement;

            var flagsAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Flags"));
            var attributeList = SyntaxFactory.AttributeList();
            attributeList = attributeList.AddAttributes(flagsAttribute);

            var newEnumDeclaration =
                enumDeclarationExpression.WithAttributeLists(enumDeclarationExpression.AttributeLists.Add(attributeList));

            var newRoot = root.ReplaceNode(statement, newEnumDeclaration);

            var compilationUnit = (CompilationUnitSyntax)newRoot;
            var semanticModel = await document.GetSemanticModelAsync();

            if (!compilationUnit.Usings.Any(usingSyntax =>
            {
                var identifier = (IdentifierNameSyntax) usingSyntax.Name;
                return identifier.Identifier.Text == "System";
            }))
            {
                var usingSystemDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));

                newRoot =
                    compilationUnit.WithUsings(compilationUnit.Usings.Add(usingSystemDirective))
                        .WithAdditionalAnnotations(Formatter.Annotation);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument.Project.Solution;
        }
    }
}