using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodWithoutAsyncSuffixAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.AsyncCategory;
        private static readonly string Message = VSDiagnosticsResources.AsyncMethodWithoutAsyncSuffixAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.AsyncMethodWithoutAsyncSuffixAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            =>
                new DiagnosticDescriptor(DiagnosticId.AsyncMethodWithoutAsyncSuffix, Title, Message, Category, Severity,
                    isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;
            if (method == null)
            {
                return;
            }

            if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return;
            }

            var returnType = context.SemanticModel.GetTypeInfo(method.ReturnType);
            if (returnType.Type == null)
            {
                return;
            }

            var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(method);
            if (declaredSymbol == null)
            {
                return;
            }

            if (declaredSymbol.IsDefinedInAncestor())
            {
                return;
            }

            if (declaredSymbol.IsAsync() && !method.Identifier.Text.EndsWith("Async"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(),
                    method.Identifier.Text));
            }
        }
    }
}