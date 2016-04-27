using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.EqualsAndGetHashcodeNotImplementedTogether
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EqualsAndGetHashcodeNotImplementedTogetherAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.EqualsAndGetHashcodeNotImplementedTogetherAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.EqualsAndGetHashcodeNotImplementedTogetherAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.EqualsAndGetHashcodeNotImplementedTogether, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var objectSymbol = context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Object);
            var objectEquals = objectSymbol.GetMembers().OfType<IMethodSymbol>().First(x => x.MetadataName == "Equals" && x.Parameters.Count() == 1);
            var objectGetHashCode = objectSymbol.GetMembers().OfType<IMethodSymbol>().First(x => x.MetadataName == "GetHashCode" && !x.Parameters.Any());
            
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            var equalsImplemented = false;
            var getHashcodeImplemented = false;

            foreach (var node in classDeclaration.Members)
            {
                if (!node.IsKind(SyntaxKind.MethodDeclaration))
                {
                    continue;
                }

                var methodDeclaration = (MethodDeclarationSyntax)node;
                if (!IsOverride(methodDeclaration))
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

            if (equalsImplemented ^ getHashcodeImplemented)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(),
                    ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string>("IsEqualsImplemented", equalsImplemented.ToString()) })));
            }
        }

        private bool IsOverride(MethodDeclarationSyntax methodDeclaration)
        {
            foreach (var modifier in methodDeclaration.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.OverrideKeyword))
                {
                    return true;
                }
            }

            return false;
        }
    }
}