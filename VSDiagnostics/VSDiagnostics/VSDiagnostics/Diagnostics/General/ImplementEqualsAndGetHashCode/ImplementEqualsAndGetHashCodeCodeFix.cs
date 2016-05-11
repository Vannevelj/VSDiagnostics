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

        private async Task<Document> ImplementEqualsAndGetHashCodeAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var model = await document.GetSemanticModelAsync();

            var objectSymbol = model.Compilation.GetSpecialType(SpecialType.System_Object);
            var objectEquals = objectSymbol.GetMembers().OfType<IMethodSymbol>()
                    .Single(method => method.MetadataName == nameof(Equals) && method.Parameters.Count() == 1);

            if (statement is ClassDeclarationSyntax)
            {
                var classDeclaration = (ClassDeclarationSyntax) statement;

                var classSymbol = model.GetDeclaredSymbol(classDeclaration);

                var newRoot = root.ReplaceNode(classDeclaration,
                    classDeclaration.AddMembers(GetEqualsMethod(model, classDeclaration.Identifier, classDeclaration.Members, classSymbol, objectEquals),
                        GetGetHashCodeMethod(model, classDeclaration.Members)));
                return document.WithSyntaxRoot(newRoot);
            }
            else
            {
                var structDeclaration = (StructDeclarationSyntax)statement;

                var structSymbol = model.GetDeclaredSymbol(structDeclaration);

                var newRoot = root.ReplaceNode(structDeclaration,
                    structDeclaration.AddMembers(GetEqualsMethod(model, structDeclaration.Identifier, structDeclaration.Members, structSymbol, objectEquals),
                        GetGetHashCodeMethod(model, structDeclaration.Members)));
                return document.WithSyntaxRoot(newRoot);
            }
        }

        private MethodDeclarationSyntax GetEqualsMethod(SemanticModel model, SyntaxToken identifier, SyntaxList<SyntaxNode> members, INamedTypeSymbol typeSymbol, IMethodSymbol objectEquals)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("obj"))
                .WithType(SyntaxFactory.ParseTypeName("object"));

            var ifStatement =
                SyntaxFactory.IfStatement(SyntaxFactory.ParseExpression($"obj == null || typeof({identifier}) != obj.GetType()"),
                    SyntaxFactory.Block(SyntaxFactory.ParseStatement("return false;")));

            // the default formatting is a single space--we want a new line
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

                    var symbol = model.GetTypeInfo(field.Declaration.Type).Type;
                    if (symbol.IsValueType)
                    {
                        fieldAndPropertyEqualityStatements.AddRange(field.Declaration.Variables.Select(
                            variable => $"{variable.Identifier}.Equals(value.{variable.Identifier})"));
                    }
                    else
                    {
                        fieldAndPropertyEqualityStatements.AddRange(field.Declaration.Variables.Select(
                            variable => $"{variable.Identifier} == value.{variable.Identifier}"));
                    }
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
                        var symbol = model.GetTypeInfo(property.Type).Type;
                        fieldAndPropertyEqualityStatements.Add(symbol.IsValueType
                            ? $"{property.Identifier}.Equals(value.{property.Identifier})"
                            : $"{property.Identifier} == value.{property.Identifier}");
                    }
                }
            }

            var symbolHasBaseTypeOverridingEquals = BaseClassImplementsEquals(objectEquals, typeSymbol);

            var returnStatement = SyntaxFactory.ParseStatement(
                    $"return {(symbolHasBaseTypeOverridingEquals ? $"base.Equals(obj) &&{Environment.NewLine}       " : string.Empty)}" +
                    string.Join($" &&{Environment.NewLine}       ", fieldAndPropertyEqualityStatements) + ";");

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "Equals")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(ifStatement, castStatement, returnStatement)
                    .AddParameterListParameters(parameter)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private MethodDeclarationSyntax GetGetHashCodeMethod(SemanticModel model, SyntaxList<SyntaxNode> members)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var fieldAndPropertyGetHashCodeStatements = new List<string>();
            foreach (var member in members)
            {
                if (member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    var field = (FieldDeclarationSyntax)member;
                    if (field.Modifiers.ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword))
                    {
                        continue;
                    }

                    var symbol = model.GetTypeInfo(field.Declaration.Type).Type;
                    if (symbol == null || !symbol.IsValueType && symbol.SpecialType != SpecialType.System_String)
                    {
                        continue;
                    }

                    if (field.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword))
                    {
                        fieldAndPropertyGetHashCodeStatements.AddRange(field.Declaration.Variables.Select(
                            variable => $"{variable.Identifier}.GetHashCode()"));
                    }
                }

                if (member.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    var property = (PropertyDeclarationSyntax)member;
                    if (property.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    var symbol = model.GetTypeInfo(property.Type).Type;
                    if (symbol == null || !symbol.IsValueType && symbol.SpecialType != SpecialType.System_String)
                    {
                        continue;
                    }

                    if (property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)))
                    {
                        continue;
                    }

                    // ensure getter does not have body
                    // the property has to have at least one of {get, set}, and it doesn't have a set (see above)
                    // this will not have an NRE in First()
                    if (property.AccessorList.Accessors.First(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)).Body == null)
                    {
                        fieldAndPropertyGetHashCodeStatements.Add($"{property.Identifier}.GetHashCode()");
                    }
                }
            }

            if (!fieldAndPropertyGetHashCodeStatements.Any())
            {
                fieldAndPropertyGetHashCodeStatements.Add("base.GetHashCode()");
            }

            var returnStatement =
                SyntaxFactory.ParseStatement("return " +
                                             string.Join($" ^{Environment.NewLine}       ",
                                                 fieldAndPropertyGetHashCodeStatements) + ";")
                    .WithLeadingTrivia(
                        SyntaxFactory.ParseLeadingTrivia(
@"// Add any fields you're interested in, taking into account the guidelines described in
// https://msdn.microsoft.com/en-us/library/system.object.gethashcode%28v=vs.110%29.aspx
"));

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("int"), "GetHashCode")
                    .AddModifiers(publicModifier, overrideModifier)
                    .AddBodyStatements(returnStatement)
                    .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private bool BaseClassImplementsEquals(IMethodSymbol objectEquals, INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind != TypeKind.Class)
            {
                return false;
            }

            var baseType = symbol.BaseType;
            if (baseType == null)
            {
                return false;
            }

            foreach (var member in baseType.GetMembers())
            {
                var method = member as IMethodSymbol;
                if (method == null || !method.IsOverride)
                {
                    continue;
                }

                while (method.IsOverride)
                {
                    method = method.OverriddenMethod;
                }

                if (method == objectEquals)
                {
                    return true; 
                }
            }

            return BaseClassImplementsEquals(objectEquals, baseType);
        }
    }
}