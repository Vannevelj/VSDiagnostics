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

        private async Task<Document> ImplementEqualsAndGetHashCodeAsync(Document document, SyntaxNode root, SyntaxNode declaration)
        {
            var model = await document.GetSemanticModelAsync();

            var objectSymbol = model.Compilation.GetSpecialType(SpecialType.System_Object);
            var objectEquals = objectSymbol.GetMembers().OfType<IMethodSymbol>()
                    .Single(method => method.MetadataName == nameof(Equals) && method.Parameters.Count() == 1);

            var typeDeclaration = (TypeDeclarationSyntax) declaration;
            var typeSymbol = model.GetDeclaredSymbol(typeDeclaration);

            var equalsMethod = GetEqualsMethod(typeDeclaration.Identifier, typeSymbol.GetMembers(), typeSymbol, objectEquals);
            var getHashCodeMethod = GetGetHashCodeMethod(typeSymbol.GetMembers());

            var newNode = declaration.IsKind(SyntaxKind.ClassDeclaration)
                ? (TypeDeclarationSyntax)((ClassDeclarationSyntax)typeDeclaration).AddMembers(equalsMethod, getHashCodeMethod)
                : (TypeDeclarationSyntax)((StructDeclarationSyntax)typeDeclaration).AddMembers(equalsMethod, getHashCodeMethod);
            
            return document.WithSyntaxRoot(root.ReplaceNode(typeDeclaration, newNode));
        }

        private MethodDeclarationSyntax GetEqualsMethod(SyntaxToken identifier, ImmutableArray<ISymbol> members, INamedTypeSymbol typeSymbol, IMethodSymbol objectEquals)
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
                var fieldEqualityStatement = GetFieldComparisonStatement(member);
                if (!string.IsNullOrEmpty(fieldEqualityStatement))
                {
                    fieldAndPropertyEqualityStatements.Add(fieldEqualityStatement);
                }

                var propertyEqualityStatement = GetPropertyComparisonStatement(member);
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

        private string GetFieldComparisonStatement(ISymbol member)
        {
            if (member.Kind != SymbolKind.Field)
            {
                return string.Empty;
            }

            var field = (IFieldSymbol)member;
            if (field.IsStatic || field.IsConst)
            {
                return string.Empty;
            }

            if (!field.DeclaringSyntaxReferences.Any())
            {
                return string.Empty;
            }
            
            return field.Type.IsValueType
                ? $"{member.Name}.Equals(value.{member.Name})"
                : $"{member.Name} == value.{member.Name}";
        }

        private string GetPropertyComparisonStatement(ISymbol member)
        {
            if (member.Kind != SymbolKind.Property)
            {
                return string.Empty;
            }
            
            var property = (IPropertySymbol)member;
            if (property.IsStatic)
            {
                return string.Empty;
            }
            
            if (property.IsWriteOnly)
            {
                return string.Empty;
            }

            return property.Type.IsValueType
                ? $"{property.Name}.Equals(value.{property.Name})"
                : $"{property.Name} == value.{property.Name}";
        }

        private MethodDeclarationSyntax GetGetHashCodeMethod(ImmutableArray<ISymbol> members)
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            var overrideModifier = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

            var fieldAndPropertyGetHashCodeStatements = new List<string>();
            foreach (var member in members)
            {
                var fieldGetHashCodeStatement = GetFieldHashCodeStatements(member);
                if (!string.IsNullOrEmpty(fieldGetHashCodeStatement))
                {
                    fieldAndPropertyGetHashCodeStatements.Add(fieldGetHashCodeStatement);
                }

                var propertyGetHashCodeStatement = GetPropertyHashCodeStatement(member);
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

        private string GetFieldHashCodeStatements(ISymbol member)
        {
            if (member.Kind != SymbolKind.Field)
            {
                return string.Empty;
            }

            var field = (IFieldSymbol) member;
            if (field.IsStatic || field.IsConst)
            {
                return string.Empty;
            }

            if (!field.DeclaringSyntaxReferences.Any())
            {
                return string.Empty;
            }

            var symbol = field.Type;
            if (!symbol.IsValueType && symbol.SpecialType != SpecialType.System_String)
            {
                return string.Empty;
            }

            if (field.IsReadOnly)
            {
                return $"{field.Name}.GetHashCode()";
            }

            return string.Empty;
        }

        private string GetPropertyHashCodeStatement(ISymbol member)
        {
            if (member.Kind != SymbolKind.Property)
            {
                return string.Empty;
            }

            var property = (IPropertySymbol)member;
            if (property.IsStatic)
            {
                return string.Empty;
            }

            var symbol = property.Type;
            if (!symbol.IsValueType && symbol.SpecialType != SpecialType.System_String)
            {
                return string.Empty;
            }

            if (!property.IsReadOnly)
            {
                return string.Empty;
            }

            var propertyNode = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences.First().GetSyntax();
            if (propertyNode.AccessorList != null &&
                propertyNode.AccessorList.Accessors.First(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)).Body == null)
            {
                return $"{property.Name}.GetHashCode()";
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