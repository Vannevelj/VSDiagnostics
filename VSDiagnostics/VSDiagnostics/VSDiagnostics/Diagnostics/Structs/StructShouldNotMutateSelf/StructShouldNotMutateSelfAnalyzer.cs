using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Structs.StructShouldNotMutateSelf
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StructShouldNotMutateSelfAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.StructsCategory;
        private static readonly string Message = VSDiagnosticsResources.StructsShouldNotMutateSelfAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.StructsShouldNotMutateSelfAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.StructShouldNotMutateSelf, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleAssignmentExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // Looking for
            // this = someValueType;
            var eq = context.Node as AssignmentExpressionSyntax;
            if (eq == null)
            {
                return;
            }

            if (!(eq.Left is ThisExpressionSyntax))
            {
                return;
            }

            var type = context.SemanticModel.GetTypeInfo(eq.Left);
            if (type.Type != null && !type.Type.IsValueType)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, eq.Left.GetLocation()));
        }
    }
}