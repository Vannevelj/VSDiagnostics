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

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SwitchStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var switchBlock = context.Node as SwitchStatementSyntax;
            if (switchBlock == null) { return; }

            var enumType = context.SemanticModel.GetTypeInfo(switchBlock.Expression).Type as INamedTypeSymbol;
            if (enumType == null || enumType.BaseType.ContainingNamespace.Name != nameof(System) || enumType.BaseType.Name != nameof(System.Enum)) { return; }

            var labelNames = switchBlock.Sections.SelectMany(l => l.Labels)
                    .OfType<CaseSwitchLabelSyntax>()
                    .Select(l => l.Value)
                    .OfType<MemberAccessExpressionSyntax>()
                    .Select(l => l.Name.Identifier.ValueText)
                    .ToList();

            if (enumType.MemberNames.Any(member => !labelNames.Contains(member)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, switchBlock.GetLocation()));
            }
        }
    }
}