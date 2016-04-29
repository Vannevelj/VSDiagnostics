using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.SwitchIsMissingDefaultLabel
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SwitchIsMissingDefaultLabelAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.SwitchIsMissingDefaultSectionAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.SwitchIsMissingDefaultSectionAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.SwitchIsMissingDefaultLabel, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SwitchStatement);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var switchBlock = (SwitchStatementSyntax)context.Node;

            var hasDefaultLabel = false;
            foreach (var section in switchBlock.Sections)
            {
                foreach (var label in section.Labels)
                {
                    if (label.IsKind(SyntaxKind.DefaultSwitchLabel))
                    {
                        hasDefaultLabel = true;
                    }
                }
            }

            if (!hasDefaultLabel)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, switchBlock.Expression.GetLocation()));
            }
        }
    }
}