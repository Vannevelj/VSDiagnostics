using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceEmptyStringWithStringDotEmptyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(ReplaceEmptyStringWithStringDotEmptyAnalyzer);
        internal const string Title = "Replaces an empty string literal with the more expressive string.Empty.";
        internal const string Message = "Empty string literal detected.";
        internal const string Category = "Strings";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StringLiteralExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.AncestorsAndSelf().OfType<AttributeArgumentSyntax>().Any())
            {
                return;
            }

            var stringLiteral = context.Node as LiteralExpressionSyntax;
            if (stringLiteral == null)
            {
                return;
            }

            if (stringLiteral.Token.Text != "\"\"")
            {
                return;
            }

            if (stringLiteral.Ancestors().Any(x => x.IsKind(SyntaxKind.Parameter)))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, stringLiteral.GetLocation()));
        }
    }
}