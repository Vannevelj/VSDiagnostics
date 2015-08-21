using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.ConditionIsAlwaysTrue
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ConditionIsAlwaysTrueAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(ConditionIsAlwaysTrueAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ConditionIsAlwaysTrueAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ConditionIsAlwaysTrueAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IfStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax) context.Node;

            if (ifStatement.Condition.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatement.Condition.GetLocation()));
            }
        }
    }
}