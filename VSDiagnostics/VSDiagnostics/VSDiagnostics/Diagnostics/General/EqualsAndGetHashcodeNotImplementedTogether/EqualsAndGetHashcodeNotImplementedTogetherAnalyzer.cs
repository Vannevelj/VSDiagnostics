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

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.EqualsAndGetHashcodeNotImplementedTogether, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterCompilationStartAction((compilationContext) => 
        {
            var objectSymbol = compilationContext.Compilation.GetSpecialType(SpecialType.System_Object);
            IMethodSymbol objectEquals = null;
            IMethodSymbol objectGetHashCode = null;

            foreach (var symbol in objectSymbol.GetMembers())
            {
                if (!(symbol is IMethodSymbol))
                {
                    continue;
                }

                var method = (IMethodSymbol)symbol;
                if (method.MetadataName == nameof(Equals) && method.Parameters.Length == 1)
                {
                    objectEquals = method;
                }

                if (method.MetadataName == nameof(GetHashCode) && !method.Parameters.Any())
                {
                    objectGetHashCode = method;
                }
            }

            compilationContext.RegisterSyntaxNodeAction((syntaxNodeContext) => 
            {
                var classDeclaration = (ClassDeclarationSyntax) syntaxNodeContext.Node;

                var equalsImplemented = false;
                var getHashcodeImplemented = false;

                foreach (var node in classDeclaration.Members)
                {
                    if (!node.IsKind(SyntaxKind.MethodDeclaration))
                    {
                        continue;
                    }

                    var methodDeclaration = (MethodDeclarationSyntax)node;
                    if (!methodDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
                    {
                        continue;
                    }

                    var methodSymbol = syntaxNodeContext.SemanticModel.GetDeclaredSymbol(methodDeclaration).OverriddenMethod;
                    
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
                    syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(),
                        ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string>("IsEqualsImplemented", equalsImplemented.ToString()) })));
                }
            }, SyntaxKind.ClassDeclaration);
        });            
    }
}