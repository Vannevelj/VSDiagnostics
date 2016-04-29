using System.Collections.Generic;
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
        static StructWithoutElementaryMethodsOverriddenCodeFix()
        {
            EqualsMethod = GetEqualsMethod();
            GetHashCodeMethod = GetGetHashCodeMethod();
            ToStringMethod = GetToStringMethod();
        }

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            string implementEqualsString;
            string implementGetHashCodeString;
            string implementToStringString;

            diagnostic.Properties.TryGetValue("IsEqualsImplemented", out implementEqualsString);
            diagnostic.Properties.TryGetValue("IsGetHashCodeImplemented", out implementGetHashCodeString);
            diagnostic.Properties.TryGetValue("IsToStringImplemented", out implementToStringString);
            
            var implementEquals = bool.Parse(implementEqualsString);
            var implementGetHashCode = bool.Parse(implementGetHashCodeString);
            var implementToString = bool.Parse(implementToStringString);

            var dict = new Dictionary<string, bool>
                {
                    {"Equals()", implementEquals},
                    {"GetHashCode()", implementGetHashCode},
                    {"ToString()", implementToString}
                };

            var statement = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(CodeAction.Create(
                string.Format(VSDiagnosticsResources.StructWithoutElementaryMethodsOverriddenCodeFixTitle, FormatMissingMembers(dict)),
                    x => AddMissingMethodsAsync(context.Document, root, (StructDeclarationSyntax) statement,
                            implementEquals, implementGetHashCode, implementToString),
                    StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.Id), diagnostic);
        }

        private static readonly MethodDeclarationSyntax EqualsMethod;
        private static readonly MethodDeclarationSyntax GetHashCodeMethod;
        private static readonly MethodDeclarationSyntax ToStringMethod;

        private Task<Document> AddMissingMethodsAsync(Document document, SyntaxNode root,
            StructDeclarationSyntax statement, bool implementEquals, bool implementGetHashCode,
            bool implementToString)
        {
            var newStatement = statement;

            if (!implementEquals)
            {
                newStatement = newStatement.AddMembers(EqualsMethod);
            }

            if (!implementGetHashCode)
            {
                newStatement = newStatement.AddMembers(GetHashCodeMethod);
            }

            if (!implementToString)
            {
                newStatement = newStatement.AddMembers(ToStringMethod);
            }

            var newRoot = root.ReplaceNode(statement, newStatement);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        private static MethodDeclarationSyntax GetEqualsMethod()
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

        private static MethodDeclarationSyntax GetGetHashCodeMethod()
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

        private static MethodDeclarationSyntax GetToStringMethod()
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

        private string FormatMissingMembers(Dictionary<string, bool> members)
        {
            // if we get this far, there are at least 1 missing members
            var missingMemberCount = 0;
            foreach (var member in members)
            {
                if (!member.Value)
                {
                    missingMemberCount++;
                }
            }

            var value = string.Empty;
            for (var i = 0; i < members.Count; i++)
            {
                if (members.ElementAt(i).Value)
                {
                    continue;
                }

                if (missingMemberCount == 2 && !string.IsNullOrEmpty(value))
                {
                    value += " and ";
                }

                value += members.ElementAt(i).Key;
                
                if (missingMemberCount == 3 && i == 0)
                {
                    value += ", ";
                }
                else if (missingMemberCount == 3 && i == 1)
                {
                    value += ", and ";
                }
            }

            return value;
        }
    }
}