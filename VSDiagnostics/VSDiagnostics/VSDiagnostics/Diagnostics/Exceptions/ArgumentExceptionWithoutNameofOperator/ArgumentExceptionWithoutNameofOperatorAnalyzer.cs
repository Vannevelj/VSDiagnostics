using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
// ReSharper disable LoopCanBeConvertedToQuery

namespace VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithoutNameofOperator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentExceptionWithoutNameofOperatorAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;

        private static readonly string Message =
            VSDiagnosticsResources.ArgumentExceptionWithoutNameofOperatorAnalyzerMessage;

        private static readonly string Title =
            VSDiagnosticsResources.ArgumentExceptionWithoutNameofOperatorAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ArgumentExceptionWithoutNameofOperator, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax) context.Node;
            if (objectCreationExpression.ArgumentList == null || !objectCreationExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            var exceptionType = objectCreationExpression.Type;
            var symbolInformation = context.SemanticModel.GetSymbolInfo(exceptionType);
            if (symbolInformation.Symbol.InheritsFrom(typeof(ArgumentException)))
            {
                var arguments = new List<LiteralExpressionSyntax>();

                foreach (var argument in objectCreationExpression.ArgumentList.Arguments)
                {
                    var expression = argument.Expression as LiteralExpressionSyntax;
                    if (expression != null)
                    {
                        arguments.Add(expression);
                    }
                }

                var methodParameters =
                    objectCreationExpression.Ancestors()
                                            .NonLinqOfType<MethodDeclarationSyntax>(SyntaxKind.MethodDeclaration)
                                            .NonLinqFirstOrDefault()?
                                            .ParameterList.Parameters;

                // Exception is declared outside a method
                if (methodParameters == null)
                {
                    return;
                }

                foreach (var argument in arguments)
                {
                    var argumentName = argument.Token.ValueText;
                    ParameterSyntax correspondingParameter = null;

                    foreach (var parameter in methodParameters.Value)
                    {
                        if (string.Equals((string) parameter.Identifier.Value, argumentName,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            correspondingParameter = parameter;
                            break;
                        }
                    }


                    if (correspondingParameter != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, argument.GetLocation(),
                            correspondingParameter.Identifier.Value));
                        return;
                    }
                }
            }
        }
    }
}