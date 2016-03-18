using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.CatchNullReferenceException
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatchingNullReferenceExceptionAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;
        private static readonly string Message = VSDiagnosticsResources.CatchNullReferenceExceptionAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.CatchNullReferenceExceptionAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.CatchingNullReferenceException, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.CatchDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var catchDeclaration = (CatchDeclarationSyntax) context.Node;

            var catchType = catchDeclaration.Type;
            if (catchType == null)
            {
                return;
            }

            var catchSymbol = context.SemanticModel.GetSymbolInfo(catchType).Symbol;
            if (catchSymbol != null)
            {
                if (catchSymbol.MetadataName == typeof (NullReferenceException).Name)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, catchDeclaration.GetLocation()));
                }
            }
        }
    }
}