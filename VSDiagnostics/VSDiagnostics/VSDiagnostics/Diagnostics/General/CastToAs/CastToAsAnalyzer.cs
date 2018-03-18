using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.CastToAs
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CastToAsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.CastToAsAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.CastToAsAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.CastToAs, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.CastExpression);

        /// <summary>
        ///     We don't handle situations like
        ///     int x = (int) o;
        ///     Turning this into a soft cast (o as int?) would cause issues when trying to apply this to a generic definition
        ///     This is only needed for non-nullable valuetypes because they need to become nullable for this kind of cast
        /// </summary>
        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax) context.Node;
            var type = context.SemanticModel.GetTypeInfo(castExpression.Type).Type;
            if (type.IsValueType && type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
    }
}