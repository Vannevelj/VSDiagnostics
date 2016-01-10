using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OnPropertyChangedWithoutNameOfOperatorAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;

        private static readonly string Message =
            VSDiagnosticsResources.OnPropertyChangedWithoutNameOfOperatorAnalyzerMessage;

        private static readonly string Title =
            VSDiagnosticsResources.OnPropertyChangedWithoutNameOfOperatorAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            =>
                new DiagnosticDescriptor(DiagnosticId.OnPropertyChangedWithoutNameofOperator, Title, Message, Category,
                    Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;

            var identifierExpression = invocation?.Expression as IdentifierNameSyntax;
            if (identifierExpression == null)
            {
                return;
            }

            var identifier = identifierExpression.Identifier;
            if (identifier.ValueText != "OnPropertyChanged")
            {
                return;
            }

            if (invocation.ArgumentList == null || !invocation.ArgumentList.Arguments.Any())
            {
                return;
            }

            var invokedProperty = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (invokedProperty == null)
            {
                return;
            }

            // We use the descendent nodes in case it's wrapped in another level. For example: OnPropertyChanged(((nameof(MyProperty))))
            var descendentInvocation = invokedProperty.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>();
            if (descendentInvocation.Any(x => x.IsNameofInvocation()))
            {
                return;
            }

            var invocationArgument = context.SemanticModel.GetConstantValue(invokedProperty.Expression);
            if (!invocationArgument.HasValue)
            {
                return;
            }

            var properties = invocation.Ancestors()
                                       .OfType<ClassDeclarationSyntax>()
                                       .FirstOrDefault()
                                       .ChildNodes()
                                       .OfType<PropertyDeclarationSyntax>();
            foreach (var property in properties)
            {
                if (string.Equals(property.Identifier.ValueText, (string) invocationArgument.Value, StringComparison.OrdinalIgnoreCase))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, invokedProperty.GetLocation(),
                        property.Identifier.ValueText));
                }
            }
        }
    }
}