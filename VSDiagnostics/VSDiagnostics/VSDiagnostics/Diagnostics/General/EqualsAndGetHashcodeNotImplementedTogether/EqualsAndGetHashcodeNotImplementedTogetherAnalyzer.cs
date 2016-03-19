using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.EqualsAndGetHashcodeNotImplementedTogether
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EqualsAndGetHashcodeNotImplemented : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.EqualsAndGetHashcodeNotImplementedTogetherAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.EqualsAndGetHashcodeNotImplementedTogetherAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.EqualsAndGetHashcodeNotImplementedTogether, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
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

                if (methodDeclaration.Identifier.ValueText == nameof(Equals) && methodDeclaration.ParameterList.Parameters.Count == 1)
                {
                    equalsImplemented = true;
                }

                if (methodDeclaration.Identifier.ValueText == nameof(GetHashCode) && methodDeclaration.ParameterList.Parameters.Count == 0)
                {
                    getHashcodeImplemented = true;
                }
            }

            if (equalsImplemented ^ getHashcodeImplemented)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.GetLocation()));
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