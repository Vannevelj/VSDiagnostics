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
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Info;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ImplementEqualsAndGetHashCodeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ImplementEqualsAndGetHashCodeAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ImplementEqualsAndGetHashCode, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ClassDeclaration);

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

            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            var equalsImplemented = false;
            var getHashcodeImplemented = false;
            var hasfieldOrProperty = false;

            foreach (var node in classDeclaration.Members)
            {
                if (node.IsKind(SyntaxKind.FieldDeclaration))
                {
                    hasfieldOrProperty = true;
                }

                if (node.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    var property = (PropertyDeclarationSyntax)node;
                    foreach (var accessor in property.AccessorList.Accessors)
                    {
                        if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                        {
                            hasfieldOrProperty = true;
                        }
                    }
                }

                if (!node.IsKind(SyntaxKind.MethodDeclaration))
                {
                    continue;
                }

                var methodDeclaration = (MethodDeclarationSyntax)node;
                if (!methodDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
                {
                    continue;
                }

                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration).OverriddenMethod;

                // this will happen if the base class is deleted and there is still a derived class
                if (methodSymbol == null)
                {
                    return;
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
                    getHashcodeImplemented = true;
                }
            }

            if (!equalsImplemented && !getHashcodeImplemented && hasfieldOrProperty)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier));
            }
        }
    }
}