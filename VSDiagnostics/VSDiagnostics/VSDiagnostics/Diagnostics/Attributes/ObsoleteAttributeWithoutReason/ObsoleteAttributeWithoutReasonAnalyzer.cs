using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.Attributes.ObsoleteAttributeWithoutReason
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ObsoleteAttributeWithoutReasonAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.AttributesCategory;
        private static readonly string Message = VSDiagnosticsResources.ObsoleteAttributeWithoutReasonAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ObsoleteAttributeWithoutReasonAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ObsoleteAttributeWithoutReason, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeCSharpSymbol, SyntaxKind.Attribute);

        private void AnalyzeCSharpSymbol(SyntaxNodeAnalysisContext context)
        {
            var attributeExpression = context.Node as AttributeSyntax;
            if (attributeExpression == null)
            {
                return;
            }

            // attribute type must be of type ObsoleteAttribute
            var type = context.SemanticModel.GetSymbolInfo(attributeExpression).Symbol;
            if (type == null || type.ContainingType.MetadataName != typeof (ObsoleteAttribute).Name)
            {
                return;
            }

            // attribute must have arguments
            // if there are no parenthesis, the ArgumentList is null
            // if there are empty parenthesis, the ArgumentList is empty
            if (attributeExpression.ArgumentList != null && attributeExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, attributeExpression.GetLocation()));
        }
    }
}