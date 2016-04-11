using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.SwitchDoesNotHandleAllEnumOptions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SwitchDoesNotHandleAllEnumOptionsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.SwitchDoesNotHandleAllEnumOptionsAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.SwitchDoesNotHandleAllEnumOptionsAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.SwitchDoesNotHandleAllEnumOptions, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SwitchStatement);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var switchBlock = (SwitchStatementSyntax) context.Node;

            var enumType = context.SemanticModel.GetTypeInfo(switchBlock.Expression).Type as INamedTypeSymbol;
            if (enumType == null || enumType.TypeKind != TypeKind.Enum)
            {
                return;
            }

            var caseLabels = new List<ExpressionSyntax>();

            foreach (var section in switchBlock.Sections)
            {
                foreach (var label in section.Labels)
                {
                    if (label.IsKind(SyntaxKind.CaseSwitchLabel))
                    {
                        caseLabels.Add(((CaseSwitchLabelSyntax) label).Value);
                    }
                }
            }

            var labelSymbols = new List<ISymbol>();
            foreach (var label in caseLabels)
            {
                labelSymbols.Add(context.SemanticModel.GetSymbolInfo(label).Symbol);
            }
            
            foreach (var member in enumType.GetMembers())
            {
                // skip `.ctor`
                if (member.Name.StartsWith("."))
                {
                    continue;
                }

                var switchHasSymbol = false;
                foreach (var symbol in labelSymbols)
                {
                    if (symbol == member)
                    {
                        switchHasSymbol = true;
                    }
                }

                if (!switchHasSymbol)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, switchBlock.GetLocation()));
                    return;
                }
            }
        }
    }
}