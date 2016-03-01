using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.SingleGeneralException
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingleGeneralExceptionAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;
        private static readonly string Message = VSDiagnosticsResources.SingleGeneralExceptionAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.SingleGeneralExceptionAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.SingleGeneralException, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.TryStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var tryStatement = context.Node as TryStatementSyntax;
            if (tryStatement?.Catches.Count != 1)
            {
                return;
            }

            var catchClause = tryStatement.Catches.First();
            var declaredException = catchClause.Declaration?.Type;
            if (declaredException == null)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(declaredException).Symbol;
            if (symbol != null)
            {
                if (symbol.MetadataName == typeof (Exception).Name)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declaredException.GetLocation()));
                }
            }
        }
    }
}