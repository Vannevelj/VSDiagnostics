using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.UsingStatementWithoutBraces
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsingStatementWithoutBracesAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.UsingStatementWithoutBracesAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.UsingStatementWithoutBracesAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.UsingStatementWithoutBraces, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.UsingStatement);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax) context.Node;

            if (!usingStatement.Statement.IsKind(SyntaxKind.UsingStatement) &&
                !usingStatement.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, usingStatement.UsingKeyword.GetLocation()));
            }
        }
    }
}