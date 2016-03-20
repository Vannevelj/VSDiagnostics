using System.Collections.Immutable;
using System.Linq;
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

            var caseLabels = switchBlock.Sections.SelectMany(l => l.Labels)
                    .OfType<CaseSwitchLabelSyntax>()
                    .Select(l => l.Value)
                    .ToList();

            var labelSymbols = caseLabels.Select(l => context.SemanticModel.GetSymbolInfo(l).Symbol);

            if (!enumType.GetMembers().Where(m => !m.Name.StartsWith(".")).SequenceEqual(labelSymbols))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, switchBlock.GetLocation()));
            }
        }
    }
}