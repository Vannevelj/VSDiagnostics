using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Arithmetic.DivideIntegerByInteger
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DivideIntegerByIntegerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly SpecialType[] IntegerTypes =
        {
            SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_Int32, SpecialType.System_Int64,
            SpecialType.System_SByte, SpecialType.System_UInt16, SpecialType.System_UInt32, SpecialType.System_UInt64
        };

        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ArithmeticCategory;
        private static readonly string Message = VSDiagnosticsResources.DivideIntegerByIntegerAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.DivideIntegerByIntegerAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            =>
                new DiagnosticDescriptor(DiagnosticId.DivideIntegerByInteger, Title, Message, Category, Severity,
                    isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.DivideExpression);

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var divideExpression = (BinaryExpressionSyntax) context.Node;
            var leftType = context.SemanticModel.GetTypeInfo(divideExpression.Left).Type;
            if (leftType == null)
            {
                return;
            }

            if (IntegerTypes.Contains(leftType.SpecialType))
            {
                var rightType = context.SemanticModel.GetTypeInfo(divideExpression.Right).Type;
                if (rightType == null)
                {
                    return;
                }

                if (IntegerTypes.Contains(rightType.SpecialType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, divideExpression.GetLocation(), divideExpression.ToString()));
                }
            }
        }
    }
}