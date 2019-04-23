using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.ThrowNull
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ThrowNullAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;
        private static readonly string Message = VSDiagnosticsResources.ThrowNullMessage;
        private static readonly string Title = VSDiagnosticsResources.ThrowNullMessage;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ThrowNull, Title, Message, Category, Severity, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ThrowStatement);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var throwStatement = (ThrowStatementSyntax) context.Node;

            var throwValue = context.SemanticModel.GetConstantValue(throwStatement.Expression);
            if (throwValue.HasValue && throwValue.Value == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, throwStatement.Expression.GetLocation()));
            }
        }
    }
}
