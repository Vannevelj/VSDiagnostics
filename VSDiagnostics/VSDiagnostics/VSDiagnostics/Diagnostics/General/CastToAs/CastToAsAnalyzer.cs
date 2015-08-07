using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.CastToAs
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CastToAsAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(CastToAsAnalyzer);
        private const string Message = "Use as instead of a cast.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Info;
        private const string Title = "You can use as instead of a cast.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.CastExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = context.Node as CastExpressionSyntax;
            if (literalExpression == null)
            {
                return;
            }

            if (context.SemanticModel.GetTypeInfo(literalExpression.Expression).ConvertedType.IsValueType)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, literalExpression.GetLocation()));
        }
    }
}