using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Exceptions.CatchNullReferenceException
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatchNullReferenceExceptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(CatchNullReferenceExceptionAnalyzer);
        internal const string Title = "Verifies whether no NullReferenceExceptions are caught.";
        internal const string Message = "A catch clause catches NullReferenceException. Consider using != null or null propagation instead.";
        internal const string Category = "Exceptions";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.CatchDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var catchDeclaration = context.Node as CatchDeclarationSyntax;
            if (catchDeclaration == null)
            {
                return;
            }

            var catchType = catchDeclaration.Type;
            if (catchType == null)
            {
                return;
            }

            var catchSymbol = context.SemanticModel.GetSymbolInfo(catchType).Symbol;
            if (catchSymbol != null)
            {
                if (catchSymbol.MetadataName == "NullReferenceException")
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, catchDeclaration.GetLocation()));
                }
            }
        }
    }
}