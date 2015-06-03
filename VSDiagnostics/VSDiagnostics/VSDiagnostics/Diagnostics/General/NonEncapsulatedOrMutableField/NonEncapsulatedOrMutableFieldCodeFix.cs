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
using Microsoft.CodeAnalysis.Formatting;
using VSDiagnostics.Diagnostics.General.NamingConventions;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField
{
    [ExportCodeFixProvider("NonEncapsulatedOrMutableField", LanguageNames.CSharp), Shared]
    public class NonEncapsulatedOrMutableFieldCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use property", x => UsePropertyAsync(context.Document, root, statement)), diagnostic);
        }

        private async Task<Solution> UsePropertyAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            // Create a new property
            // Using property naming conventions
            // Including possible initializers
            // And attributes

            var variableDeclarator = statement.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
            var fieldStatement = variableDeclarator.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
            var variableDeclaration = variableDeclarator.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();

            var newProperty = SyntaxFactory.PropertyDeclaration(variableDeclaration.Type, variableDeclarator.Identifier.WithConvention(NamingConvention.UpperCamelCase))
                                           .WithAttributeLists(fieldStatement.AttributeLists)
                                           .WithModifiers(fieldStatement.Modifiers)
                                           .WithAdditionalAnnotations(Formatter.Annotation)
                                           .WithAccessorList(
                                               SyntaxFactory.AccessorList(
                                                   SyntaxFactory.List(new[]
                                                   {
                                                       SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                                       SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                                   })));

            if (variableDeclarator.Initializer != null)
            {
                newProperty = newProperty.WithInitializer(variableDeclarator.Initializer).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }

            var editor = await DocumentEditor.CreateAsync(document);
            editor.InsertAfter(statement, newProperty);
            editor.RemoveNode(variableDeclarator);
            return editor.GetChangedDocument().Project.Solution;
        }
    }
}