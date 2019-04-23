using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.DateTimeNow
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeNowAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.DateTimeNowAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.DateTimeNowAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.DateTimeNow, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SimpleMemberAccessExpression);

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var expression = (MemberAccessExpressionSyntax)context.Node;
            var symbol = context.SemanticModel.GetSymbolInfo(expression.Expression).Symbol as INamedTypeSymbol;

            if (symbol != null &&
                symbol.SpecialType == SpecialType.System_DateTime &&
                expression.Name.Identifier.ValueText == "Now")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}
