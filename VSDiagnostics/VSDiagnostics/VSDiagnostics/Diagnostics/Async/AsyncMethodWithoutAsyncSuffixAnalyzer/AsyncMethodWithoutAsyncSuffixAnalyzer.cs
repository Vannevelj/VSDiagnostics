using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffixAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodWithoutAsyncSuffixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(AsyncMethodWithoutAsyncSuffixAnalyzer);
        internal const string Title = "Verifies whether an async method has the 'Async' suffix.";
        internal const string Message = "Method \"{0}\" does not end with 'Async'.";
        internal const string Category = "Async";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
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

            if (method.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (!method.Identifier.Text.EndsWith("Async"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, method.GetLocation(), method.Identifier.Text));
                }
            }
        }
    }
}