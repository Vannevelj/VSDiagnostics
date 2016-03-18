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

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleAssignmentExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // Looking for
            // this = someValueType;
            var assignmentExpression = context.Node as AssignmentExpressionSyntax;
            if (assignmentExpression == null)
            {
                return;
            }

            if (!(assignmentExpression.Left is ThisExpressionSyntax))
            {
                return;
            }

            var type = context.SemanticModel.GetTypeInfo(assignmentExpression.Left).Type;
            if (type == null)
            {
                return;
            }

            if (!type.IsValueType)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, assignmentExpression.Left.GetLocation(), type.Name));
        }
    }
}