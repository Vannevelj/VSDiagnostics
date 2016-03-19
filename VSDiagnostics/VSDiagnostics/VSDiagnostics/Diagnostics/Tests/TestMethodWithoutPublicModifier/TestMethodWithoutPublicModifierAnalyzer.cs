using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifier
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestMethodWithoutPublicModifierAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.TestsCategory;
        private static readonly string Message = VSDiagnosticsResources.TestMethodWithoutPublicModifierAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.TestMethodWithoutPublicModifierAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.TestMethodWithoutPublicModifier, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax) context.Node;

            if (IsTestMethod(method) && !method.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(),
                    method.Identifier.Text));
            }
        }

        private static bool IsTestMethod(MethodDeclarationSyntax method)
        {
            var methodAttributes = new[] { "Test", "TestMethod", "Fact" };
            var attributes = method.AttributeLists.FirstOrDefault()?.Attributes;

            if (attributes == null)
            {
                return false;
            }

            return attributes.Value.Any(x => methodAttributes.Contains(x.Name.ToString()));
        }
    }
}