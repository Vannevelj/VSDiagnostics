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

            var typeDeclaration = (TypeDeclarationSyntax) statement;
            var typeSymbol = model.GetDeclaredSymbol(typeDeclaration);

            var equalsMethod = GetEqualsMethod(model, typeDeclaration.Identifier, typeDeclaration.Members, typeSymbol, objectEquals);
            var getHashCodeMethod = GetGetHashCodeMethod(model, typeDeclaration.Members);

            var newNode = statement.IsKind(SyntaxKind.ClassDeclaration)
                ? (TypeDeclarationSyntax)((ClassDeclarationSyntax)typeDeclaration).AddMembers(equalsMethod, getHashCodeMethod)
                : (TypeDeclarationSyntax)((StructDeclarationSyntax)typeDeclaration).AddMembers(equalsMethod, getHashCodeMethod);
            
            return document.WithSyntaxRoot(root.ReplaceNode(typeDeclaration, newNode));
        }

        private MethodDeclarationSyntax GetEqualsMethod(SemanticModel model, SyntaxToken identifier, SyntaxList<SyntaxNode> members, INamedTypeSymbol typeSymbol, IMethodSymbol objectEquals)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("obj"))
                .WithType(SyntaxFactory.ParseTypeName("object"));

            var ifStatement = SyntaxFactory.IfStatement(SyntaxFactory.ParseExpression($"obj == null || typeof({identifier}) != obj.GetType()"),
                    SyntaxFactory.Block(SyntaxFactory.ParseStatement("return false;")));

            // the default formatting is a single space--we want a new line
            var castStatement = SyntaxFactory.ParseStatement($"var value = ({identifier}) obj;{Environment.NewLine}");

            var fieldAndPropertyEqualityStatements = new List<string>();
            foreach (var member in members)
            {
                fieldAndPropertyEqualityStatements.AddRange(GetFieldComparisonStatements(model, member));

                var propertyEqualityStatement = GetPropertyComparisonStatement(model, member);
                if (!string.IsNullOrEmpty(propertyEqualityStatement))
                {
                    fieldAndPropertyEqualityStatements.Add(propertyEqualityStatement);
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

        private IEnumerable<string> GetFieldComparisonStatements(SemanticModel model, SyntaxNode member)
        {
            if (!member.IsKind(SyntaxKind.FieldDeclaration))
            {
                return Enumerable.Empty<string>();
            }

            var field = (FieldDeclarationSyntax)member;
            if (field.Modifiers.ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword))
            {
                return Enumerable.Empty<string>();
            }

            var symbol = model.GetTypeInfo(field.Declaration.Type).Type;
            return symbol.IsValueType
                ? field.Declaration.Variables.Select(variable => $"{variable.Identifier}.Equals(value.{variable.Identifier})")
                : field.Declaration.Variables.Select(variable => $"{variable.Identifier} == value.{variable.Identifier}");
        }

        private string GetPropertyComparisonStatement(SemanticModel model, SyntaxNode member)
        {
            if (!member.IsKind(SyntaxKind.PropertyDeclaration))
            {
                return string.Empty;
            }

            var property = (PropertyDeclarationSyntax)member;
            if (property.Modifiers.Contains(SyntaxKind.StaticKeyword) ||
                !property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)))
            {
                return string.Empty;
            }

            var symbol = model.GetTypeInfo(property.Type).Type;
            return symbol.IsValueType
                ? $"{property.Identifier}.Equals(value.{property.Identifier})"
                : $"{property.Identifier} == value.{property.Identifier}";
        }

        private MethodDeclarationSyntax GetGetHashCodeMethod(SemanticModel model, SyntaxList<SyntaxNode> members)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var fieldAndPropertyGetHashCodeStatements = new List<string>();
            foreach (var member in members)
            {
                fieldAndPropertyGetHashCodeStatements.AddRange(GetFieldHashCodeStatements(model, member));

                var propertyGetHashCodeStatement = GetPropertyHashCodeStatement(model, member);
                if (!string.IsNullOrEmpty(propertyGetHashCodeStatement))
                {
                    fieldAndPropertyGetHashCodeStatements.Add(propertyGetHashCodeStatement);
                }
            }

            if (!fieldAndPropertyGetHashCodeStatements.Any())
            {
                fieldAndPropertyGetHashCodeStatements.Add("base.GetHashCode()");
            }

            var returnStatement = SyntaxFactory.ParseStatement("return " +
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

        private IEnumerable<string> GetFieldHashCodeStatements(SemanticModel model, SyntaxNode member)
        {
            if (!member.IsKind(SyntaxKind.FieldDeclaration))
            {
                return Enumerable.Empty<string>();
            }

            var field = (FieldDeclarationSyntax) member;
            if (field.Modifiers.ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword))
            {
                return Enumerable.Empty<string>();
            }

            var symbol = model.GetTypeInfo(field.Declaration.Type).Type;
            if (symbol == null || !symbol.IsValueType && symbol.SpecialType != SpecialType.System_String)
            {
                return Enumerable.Empty<string>();
            }

            if (field.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword))
            {
                return field.Declaration.Variables.Select(
                    variable => $"{variable.Identifier}.GetHashCode()");
            }

            return Enumerable.Empty<string>();
        }

        private string GetPropertyHashCodeStatement(SemanticModel model, SyntaxNode member)
        {
            if (!member.IsKind(SyntaxKind.PropertyDeclaration))
            {
                return string.Empty;
            }

            var property = (PropertyDeclarationSyntax)member;
            if (property.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return string.Empty;
            }

            var symbol = model.GetTypeInfo(property.Type).Type;
            if (symbol == null || !symbol.IsValueType && symbol.SpecialType != SpecialType.System_String)
            {
                return string.Empty;
            }

            if (property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)))
            {
                return string.Empty;
            }

            // ensure getter does not have body
            // the property has to have at least one of {get, set}, and it doesn't have a set (see above)
            // this will not have an NRE in First()
            if (property.AccessorList.Accessors.First(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)).Body == null)
            {
                return $"{property.Identifier}.GetHashCode()";
            }

            return string.Empty;
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