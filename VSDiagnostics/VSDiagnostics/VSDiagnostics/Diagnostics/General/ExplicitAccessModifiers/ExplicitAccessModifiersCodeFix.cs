using System.Collections.Immutable;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.ExplicitAccessModifiers
{
    [ExportCodeFixProvider("ExplicitAccessModifiers", LanguageNames.CSharp), Shared]
    public class ExplicitAccessModifiersCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ExplicitAccessModifiersAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);

            var semanticModel = context.Document.GetSemanticModelAsync().Result;
            var symbol = semanticModel.GetDeclaredSymbol(statement);
            var accessibility = symbol?.DeclaredAccessibility ?? Accessibility.Private;

            context.RegisterCodeFix(CodeAction.Create("Add Modifier", x => AddModifier(context.Document, root, statement, accessibility), nameof(ExplicitAccessModifiersAnalyzer)), diagnostic);
        }

        private Task<Solution> AddModifier(Document document, SyntaxNode root, SyntaxNode statement, Accessibility accessibility)
        {
            SyntaxNode newStatement = null;
            var accessModifiers = new List<SyntaxToken>();

            switch (accessibility)
            {
                case Accessibility.Private:
                    accessModifiers.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    break;
                case Accessibility.ProtectedAndInternal:
                    accessModifiers.AddRange(new[]
                    {SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword)});
                    break;
                case Accessibility.Protected:
                    accessModifiers.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case Accessibility.Internal:
                    accessModifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case Accessibility.Public:
                    accessModifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
            }
            
            if (statement is ClassDeclarationSyntax)
            {
                var classExpression = (ClassDeclarationSyntax) statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newClass = classExpression.WithModifiers(classExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newClass);
            }

            if (statement is StructDeclarationSyntax)
            {
                var structExpression = (StructDeclarationSyntax)statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newStruct = structExpression.WithModifiers(structExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newStruct);
            }

            if (statement is EnumDeclarationSyntax)
            {
                var enumExpression = (EnumDeclarationSyntax)statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newStruct = enumExpression.WithModifiers(enumExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newStruct);
            }

            if (statement is DelegateDeclarationSyntax)
            {
                var delegateExpression = (DelegateDeclarationSyntax)statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newStruct = delegateExpression.WithModifiers(delegateExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newStruct);
            }

            if (statement is InterfaceDeclarationSyntax)
            {
                var interfaceExpression = (InterfaceDeclarationSyntax)statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newStruct = interfaceExpression.WithModifiers(interfaceExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newStruct);
            }

            if (statement is FieldDeclarationSyntax)
            {
                var fieldExpression = (FieldDeclarationSyntax)statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newStruct = fieldExpression.WithModifiers(fieldExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newStruct);
            }

            if (statement is PropertyDeclarationSyntax)
            {
                var fieldExpression = (PropertyDeclarationSyntax)statement;
                var accessModifierTokens = SyntaxFactory.TokenList(accessModifiers);

                var newStruct = fieldExpression.WithModifiers(fieldExpression.Modifiers.AddRange(accessModifierTokens));
                newStatement = statement.ReplaceNode(statement, newStruct);
            }

            var newRoot = newStatement == null ? root : root.ReplaceNode(statement, newStatement);
            return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
        }
    }
}