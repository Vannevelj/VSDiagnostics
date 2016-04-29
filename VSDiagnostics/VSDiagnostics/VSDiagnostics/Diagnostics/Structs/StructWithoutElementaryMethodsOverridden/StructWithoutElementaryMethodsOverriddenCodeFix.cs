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

namespace VSDiagnostics.Diagnostics.Structs.StructWithoutElementaryMethodsOverridden
{
    [ExportCodeFixProvider(DiagnosticId.StructWithoutElementaryMethodsOverridden + "CF", LanguageNames.CSharp), Shared]
    public class StructWithoutElementaryMethodsOverriddenCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            string implementEquals;
            string implementGetHashCode;
            string implementToString;

            diagnostic.Properties.TryGetValue("IsEqualsImplemented", out implementEquals);
            diagnostic.Properties.TryGetValue("IsGetHashCodeImplemented", out implementGetHashCode);
            diagnostic.Properties.TryGetValue("IsToStringImplemented", out implementToString);

            var statement = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.ReplaceEmptyStringWithStringDotEmptyCodeFixTitle,
                    x => AddMissingMethodsAsync(context.Document, root, (StructDeclarationSyntax) statement,
                            bool.Parse(implementEquals), bool.Parse(implementGetHashCode), bool.Parse(implementToString)),
                    StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Document> AddMissingMethodsAsync(Document document, SyntaxNode root,
            StructDeclarationSyntax statement, bool implementEquals, bool implementGetHashCode,
            bool implementToString)
        {
            var newStatement = statement;

            if (!implementEquals)
            {
                newStatement = newStatement.AddMembers(GetEqualsMethod());
            }

            if (!implementGetHashCode)
            {
                newStatement = newStatement.AddMembers(GetGetHashCodeMethod());
            }

            if (!implementToString)
            {
                newStatement = newStatement.AddMembers(GetToStringMethod());
            }

            var newRoot = root.ReplaceNode(statement, newStatement);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
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

        private MethodDeclarationSyntax GetToStringMethod()
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);
            var bodyStatement = SyntaxFactory.ParseStatement("throw new System.NotImplementedException();")
                .WithAdditionalAnnotations(Simplifier.Annotation);

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("string"), "ToString")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(bodyStatement)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}