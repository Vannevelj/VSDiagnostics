using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ConditionIsConstant
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ConditionIsConstant : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ConditionIsConstantAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ConditionIsConstantAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ConditionIsConstant, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IfStatement);

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
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    ifStatement.Condition.GetLocation(),
                    ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string>("IsConditionTrue", "true") }),
                    "true"));
            }

            if (!(bool)constantValue.Value)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    ifStatement.Condition.GetLocation(),
                    ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string>("IsConditionTrue", "false") }),
                    "false"));
            }
        }
    }
}