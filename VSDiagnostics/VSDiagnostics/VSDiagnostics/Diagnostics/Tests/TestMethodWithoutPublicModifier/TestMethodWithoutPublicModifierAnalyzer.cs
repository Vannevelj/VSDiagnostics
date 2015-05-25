using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifier
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestMethodWithoutPublicModifierAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(TestMethodWithoutPublicModifierAnalyzer);
        internal const string Title = "Verifies whether a test method has the public modifier.";
        internal const string Message = "Test method \"{0}\" is not public.";
        internal const string Category = "Tests";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;
            if (method == null)
            {
                return;
            }

            if (IsTestMethod(method))
            {
                if (!method.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
                }
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