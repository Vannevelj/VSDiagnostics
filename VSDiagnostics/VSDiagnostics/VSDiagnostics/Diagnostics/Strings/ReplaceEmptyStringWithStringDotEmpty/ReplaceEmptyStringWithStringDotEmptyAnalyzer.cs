using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceEmptyStringWithStringDotEmptyAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.StringsCategory;

        private static readonly string Message =
            VSDiagnosticsResources.ReplaceEmptyStringWithStringDotEmptyAnalyzerMessage;

        private static readonly string Title = VSDiagnosticsResources.ReplaceEmptyStringWithStringDotEmptyAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ReplaceEmptyStringWithStringDotEmpty, Title, Message, Category, Severity, true);

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

            var variableDeclaration = stringLiteral.Ancestors().OfType<FieldDeclarationSyntax>().FirstOrDefault();
            if (variableDeclaration != null)
            {
                if (variableDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.ConstKeyword)))
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, stringLiteral.GetLocation()));
        }
    }
}