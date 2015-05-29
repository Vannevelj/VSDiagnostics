using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OnPropertyChangedWithoutNameOfOperatorAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(OnPropertyChangedWithoutNameOfOperatorAnalyzer);
        internal const string Title = "Use the nameof() operator in conjunection with OnPropertyChanged";
        internal const string Message = "OnPropertyChanged({0}) can use the nameof() operator.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null)
            {
                return;
            }

            var identifierExpression = invocation.Expression as IdentifierNameSyntax;
            if (identifierExpression == null)
            {
                return;
            }

            var identifier = identifierExpression.Identifier;
            if (identifier.ValueText != "OnPropertyChanged")
            {
                return;
            }

            var invokedProperty = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (invokedProperty == null)
            {
                return;
            }

            var argumentLiteralExpression = invokedProperty.Expression as LiteralExpressionSyntax;
            if (argumentLiteralExpression == null)
            {
                return;
            }

            var invocationArgument = argumentLiteralExpression.Token.ValueText;

            var properties = invocation.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault().ChildNodes().OfType<PropertyDeclarationSyntax>();
            foreach (var property in properties)
            {
                if (string.Equals(property.Identifier.ValueText, invocationArgument, StringComparison.OrdinalIgnoreCase))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, invokedProperty.GetLocation(), property.Identifier.ValueText));
                }
            }
        }
    }
}