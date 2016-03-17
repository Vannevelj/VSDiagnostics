﻿using System.Collections.Immutable;
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

namespace VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField
{
    [ExportCodeFixProvider(nameof(NonEncapsulatedOrMutableFieldCodeFix), LanguageNames.CSharp), Shared]
    public class NonEncapsulatedOrMutableFieldCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NonEncapsulatedOrMutableFieldAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.NonEncapsulatedOrMutableFieldCodeFixTitle,
                    x => UsePropertyAsync(context.Document, statement), NonEncapsulatedOrMutableFieldAnalyzer.Rule.Id),
                diagnostic);
        }

        private async Task<Solution> UsePropertyAsync(Document document, SyntaxNode statement)
        {
            // Create a new property
            // Using property naming conventions
            // Including possible initializers
            // And attributes

            var variableDeclarator = statement.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
            var fieldStatement = variableDeclarator.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
            var variableDeclaration = variableDeclarator.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();

            var newProperty = SyntaxFactory.PropertyDeclaration(variableDeclaration.Type, variableDeclarator.Identifier)
                .WithAttributeLists(fieldStatement.AttributeLists)
                .WithModifiers(fieldStatement.Modifiers)
                .WithAdditionalAnnotations(Formatter.Annotation)
                .WithAccessorList(
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.List(new[]
                        {
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                        })));

            if (variableDeclarator.Initializer != null)
            {
                newProperty =
                    newProperty.WithInitializer(variableDeclarator.Initializer)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }

            var editor = await DocumentEditor.CreateAsync(document);
            editor.InsertAfter(statement, newProperty);
            editor.RemoveNode(variableDeclarator);
            return editor.GetChangedDocument().Project.Solution;
        }
    }
}