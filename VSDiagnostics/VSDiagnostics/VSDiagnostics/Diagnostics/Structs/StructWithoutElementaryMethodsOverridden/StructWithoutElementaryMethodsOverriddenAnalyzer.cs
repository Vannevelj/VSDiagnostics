using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Structs.StructWithoutElementaryMethodsOverridden
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StructWithoutElementaryMethodsOverriddenAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.StructsCategory;
        private static readonly string Message = VSDiagnosticsResources.StructWithoutElementaryMethodsOverriddenAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.StructWithoutElementaryMethodsOverriddenAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.StructWithoutElementaryMethodsOverridden, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.StructDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var objectSymbol = context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Object);
            IMethodSymbol objectEquals = null;
            IMethodSymbol objectGetHashCode = null;
            IMethodSymbol objectToString = null;

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

                if (method.MetadataName == nameof(ToString) && !method.Parameters.Any())
                {
                    objectToString = method;
                }
            }

            var structDeclaration = (StructDeclarationSyntax)context.Node;

            var equalsImplemented = false;
            var getHashcodeImplemented = false;
            var getToStringImplemented = false;

            foreach (var node in structDeclaration.Members)
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

                if (methodSymbol == objectToString)
                {
                    getToStringImplemented = true;
                }
            }

            if (!equalsImplemented || !getHashcodeImplemented || !getToStringImplemented)
            {
                var isEqualsImplemented = new KeyValuePair<string, string>("IsEqualsImplemented", equalsImplemented.ToString());
                var isGetHashcodeImplemented = new KeyValuePair<string, string>("IsGetHashcodeImplemented", equalsImplemented.ToString());
                var isGetToStringImplemented = new KeyValuePair<string, string>("IsGetToStringImplemented", equalsImplemented.ToString());

                var properties = ImmutableDictionary.CreateRange(new[]
                    {isEqualsImplemented, isGetHashcodeImplemented, isGetToStringImplemented});

                context.ReportDiagnostic(Diagnostic.Create(Rule, structDeclaration.Identifier.GetLocation(), properties));
            }
        }
    }
}