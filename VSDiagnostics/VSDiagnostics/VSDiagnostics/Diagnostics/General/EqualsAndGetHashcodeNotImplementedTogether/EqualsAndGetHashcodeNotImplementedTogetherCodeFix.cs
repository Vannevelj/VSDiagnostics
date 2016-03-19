using System;
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
using Microsoft.CodeAnalysis.Simplification;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.EqualsAndGetHashcodeNotImplementedTogether
{
    [ExportCodeFixProvider(DiagnosticId.EqualsAndGetHashcodeNotImplementedTogether + "CF", LanguageNames.CSharp), Shared]
    public class EqualsAndGetHashcodeNotImplementedTogetherCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(EqualsAndGetHashcodeNotImplementedTogetherAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);

            if (bool.Parse(diagnostic.Properties["IsEqualsImplemented"]))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(string.Format(VSDiagnosticsResources.EqualsAndGetHashcodeNotImplementedTogetherCodeFixTitle, "GetHashCode()"),
                        x => ImplementGetHashCodeAsync(context.Document, root, statement),
                        EqualsAndGetHashcodeNotImplementedTogetherAnalyzer.Rule.Id),
                    diagnostic);
            }
            else
            {
                context.RegisterCodeFix(
                    CodeAction.Create(string.Format(VSDiagnosticsResources.EqualsAndGetHashcodeNotImplementedTogetherCodeFixTitle, "Equals(object obj)"),
                        x => ImplementEqualsAsync(context.Document, root, statement),
                        EqualsAndGetHashcodeNotImplementedTogetherAnalyzer.Rule.Id),
                    diagnostic);
            }
        }

        private Task<Solution> ImplementEqualsAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            try
            {
                var classDeclaration = (ClassDeclarationSyntax) statement;

                var newRoot = root.ReplaceNode(classDeclaration, classDeclaration.AddMembers(GetEqualsMethod()));
                var newDocument = document.WithSyntaxRoot(newRoot); //await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot));
                return Task.FromResult(newDocument.Project.Solution);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<Solution> ImplementGetHashCodeAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var classDeclaration = (ClassDeclarationSyntax)statement;

            var newRoot = root.ReplaceNode(classDeclaration, classDeclaration.AddMembers(GetGetHashCodeMethod()));
            var newDocument = await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot));
            return newDocument.Project.Solution;
        }

        private MethodDeclarationSyntax GetEqualsMethod()
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);
            var bodyStatement = SyntaxFactory.ParseStatement("throw new System.NotImplementedException();")
                .WithAdditionalAnnotations(Simplifier.Annotation);
            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("obj"))
                .WithType(SyntaxFactory.ParseTypeName("object"));

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "Equals")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(bodyStatement)
                    .AddParameterListParameters(parameter)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private MethodDeclarationSyntax GetGetHashCodeMethod()
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);
            var bodyStatement = SyntaxFactory.ParseStatement("throw new System.NotImplementedException();")
                .WithAdditionalAnnotations(Simplifier.Annotation);

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("int"), "GetHashCode")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(bodyStatement)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}