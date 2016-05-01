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
using VSDiagnostics.Utilities;
using System.Collections.Generic;

namespace VSDiagnostics.Diagnostics.General.ImplementEqualsAndGetHashCode
{
    [ExportCodeFixProvider(DiagnosticId.ImplementEqualsAndGetHashCode + "CF", LanguageNames.CSharp), Shared]
    public class ImplementEqualsAndGetHashCodeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ImplementEqualsAndGetHashCodeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(string.Format(VSDiagnosticsResources.ImplementEqualsAndGetHashCodeCodeFixTitle),
                    x => ImplementEqualsAndGetHashCodeAsync(context.Document, root, statement),
                    ImplementEqualsAndGetHashCodeAnalyzer.Rule.Id),
                diagnostic);
        }

        private Task<Document> ImplementEqualsAndGetHashCodeAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            if (statement is ClassDeclarationSyntax)
            {
                var classDeclaration = (ClassDeclarationSyntax) statement;

                var newRoot = root.ReplaceNode(classDeclaration,
                    classDeclaration.AddMembers(GetEqualsMethod(classDeclaration.Identifier, classDeclaration.Members),
                        GetGetHashCodeMethod(classDeclaration.Members)));
                return Task.FromResult(document.WithSyntaxRoot(newRoot));
            }
            else
            {
                var structDeclaration = (StructDeclarationSyntax)statement;

                var newRoot = root.ReplaceNode(structDeclaration,
                    structDeclaration.AddMembers(GetEqualsMethod(structDeclaration.Identifier, structDeclaration.Members),
                        GetGetHashCodeMethod(structDeclaration.Members)));
                return Task.FromResult(document.WithSyntaxRoot(newRoot));
            }
        }

        private MethodDeclarationSyntax GetEqualsMethod(SyntaxToken identifier, SyntaxList<SyntaxNode> members)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("obj"))
                .WithType(SyntaxFactory.ParseTypeName("object"));

            var ifStatement =
                SyntaxFactory.IfStatement(SyntaxFactory.ParseExpression($"obj == null || typeof({identifier}) != obj.GetType()"),
                    SyntaxFactory.Block(SyntaxFactory.ParseStatement("return false;")));

            var castStatement = SyntaxFactory.ParseStatement(
                    $"var value = ({identifier}) obj;{Environment.NewLine}");

            var fieldAndPropertyEqualityStatements = new List<string>();
            foreach (var member in members)
            {
                if (member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    var field = (FieldDeclarationSyntax)member;
                    if (field.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    fieldAndPropertyEqualityStatements.AddRange(field.Declaration.Variables.Select(
                            variable => $"{variable.Identifier} == value.{variable.Identifier}"));
                }

                if (member.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    var property = (PropertyDeclarationSyntax)member;
                    if (property.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    if (property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)))
                    {
                        fieldAndPropertyEqualityStatements.Add($"{property.Identifier} == value.{property.Identifier}");
                    }
                }
            }

            var returnStatement =
                SyntaxFactory.ParseStatement("return " + string.Join($" && ", fieldAndPropertyEqualityStatements) + ";");

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "Equals")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(ifStatement, castStatement, returnStatement)
                    .AddParameterListParameters(parameter)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private MethodDeclarationSyntax GetGetHashCodeMethod(SyntaxList<SyntaxNode> members)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var fieldAndPropertyGetHashCodeStatements = new List<string>();
            foreach (var member in members)
            {
                if (member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    var field = (FieldDeclarationSyntax)member;
                    if (field.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    fieldAndPropertyGetHashCodeStatements.AddRange(field.Declaration.Variables.Select(
                            variable => $"{variable.Identifier}.GetHashCode()"));
                }

                if (member.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    var property = (PropertyDeclarationSyntax)member;
                    if (property.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    if (property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)))
                    {
                        fieldAndPropertyGetHashCodeStatements.Add($"{property.Identifier}.GetHashCode()");
                    }
                }
            }

            var returnStatement =
                SyntaxFactory.ParseStatement("return " + string.Join(" ^ ", fieldAndPropertyGetHashCodeStatements) + ";");

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("int"), "GetHashCode")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(returnStatement)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}