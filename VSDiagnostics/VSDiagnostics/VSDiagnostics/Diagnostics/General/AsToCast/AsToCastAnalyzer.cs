using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.AsToCast
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsToCastAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.AsToCastAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.AsToCastAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.AsToCast, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.AsExpression);

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context) => context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
    }
}