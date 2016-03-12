using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ConditionIsAlwaysTrue
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ConditionIsAlwaysTrueAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ConditionIsAlwaysTrueAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ConditionIsAlwaysTrueAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ConditionIsAlwaysTrue, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IfStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax) context.Node;

            var constantValue = context.SemanticModel.GetConstantValue(ifStatement.Condition);

            if (!constantValue.HasValue)
            {
                return;
            }

            if ((bool) constantValue.Value)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatement.Condition.GetLocation()));
            }
        }
    }
}