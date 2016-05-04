using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ImplementEqualsAndGetHashCode
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImplementEqualsAndGetHashCodeAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ImplementEqualsAndGetHashCodeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ImplementEqualsAndGetHashCodeAnalyzerTitle;

        internal static DiagnosticDescriptor Rule =>
            new DiagnosticDescriptor(DiagnosticId.ImplementEqualsAndGetHashCode, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) =>
                context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var objectSymbol = context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Object);
            IMethodSymbol objectEquals = null;
            IMethodSymbol objectGetHashCode = null;

            foreach (var symbol in objectSymbol.GetMembers())
            {
                if (!(symbol is IMethodSymbol))
                {
                    continue;
                }

                var method = (IMethodSymbol)symbol;
                if (method.MetadataName == nameof(Equals) && method.Parameters.Count() == 1)
                {
                    objectEquals = method;
                }

                if (method.MetadataName == nameof(GetHashCode) && !method.Parameters.Any())
                {
                    objectGetHashCode = method;
                }
            }

            if (context.Node is ClassDeclarationSyntax)
            {
                var classDeclaration = (ClassDeclarationSyntax) context.Node;

                if (MembersDoNotContainOverridenEqualsAndGetHashCode(context.SemanticModel, classDeclaration.Members, objectEquals, objectGetHashCode) &&
                    MembersContainNonStaticFieldOrProperty(classDeclaration.Members) &&
                    MembersContainReadonlyOrValueSemanticFieldOrProperty(context.SemanticModel, classDeclaration.Members))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier));
                }
            }
            else
            {
                var structDeclaration = (StructDeclarationSyntax)context.Node;

                if (MembersDoNotContainOverridenEqualsAndGetHashCode(context.SemanticModel, structDeclaration.Members, objectEquals, objectGetHashCode) &&
                    MembersContainNonStaticFieldOrProperty(structDeclaration.Members) &&
                    MembersContainReadonlyOrValueSemanticFieldOrProperty(context.SemanticModel, structDeclaration.Members))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, structDeclaration.Identifier.GetLocation(), structDeclaration.Identifier));
                }
            }
        }

        private bool MembersDoNotContainOverridenEqualsAndGetHashCode(SemanticModel model, SyntaxList<SyntaxNode> members, IMethodSymbol objectEquals, IMethodSymbol objectGetHashCode)
        {
            var equalsImplemented = false;
            var getHashCodeImplemented = false;

            foreach (var node in members)
            {
                if (!node.IsKind(SyntaxKind.MethodDeclaration))
                {
                    continue;
                }

                var methodDeclaration = (MethodDeclarationSyntax) node;
                if (!methodDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
                {
                    continue;
                }

                var methodSymbol = model.GetDeclaredSymbol(methodDeclaration).OverriddenMethod;

                // this will happen if the base class is deleted and there is still a derived class
                if (methodSymbol == null)
                {
                    return false;    // well, technically, it doesn't exist
                }

                while (methodSymbol.IsOverride)
                {
                    methodSymbol = methodSymbol.OverriddenMethod;
                }

                if (methodSymbol == objectEquals)
                {
                    equalsImplemented = true;
                }

                if (methodSymbol == objectGetHashCode)
                {
                    getHashCodeImplemented = true;
                }
            }

            return !equalsImplemented && !getHashCodeImplemented;
        }

        private bool MembersContainNonStaticFieldOrProperty(SyntaxList<SyntaxNode> members)
        {
            foreach (var node in members)
            {
                if (node.IsKind(SyntaxKind.FieldDeclaration))
                {
                    var field = (FieldDeclarationSyntax)node;
                    if (!field.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        return true;
                    }
                }

                if (node.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    var property = (PropertyDeclarationSyntax) node;
                    if (!property.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    {
                        foreach (var accessor in property.AccessorList.Accessors)
                        {
                            if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool MembersContainReadonlyOrValueSemanticFieldOrProperty(SemanticModel model, SyntaxList<SyntaxNode> members)
        {
            foreach (var node in members)
            {
                if (node.IsKind(SyntaxKind.FieldDeclaration))
                {
                    var field = (FieldDeclarationSyntax) node;
                    if (field.Modifiers.Contains(SyntaxKind.ConstKeyword))
                    {
                        continue;
                    }

                    var symbol = model.GetTypeInfo(field.Declaration.Type).Type;
                    if (field.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword) ||
                        (symbol != null && symbol.IsValueType))
                    {
                        return true;
                    }
                }

                if (node.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    var property = (PropertyDeclarationSyntax)node;
                    var containsSetAccessor = false;

                    foreach (var accessor in property.AccessorList.Accessors)
                    {
                        if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                        {
                            containsSetAccessor = true;
                        }
                    }

                    if (!containsSetAccessor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}