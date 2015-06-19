using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithoutNameofOperator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentExceptionWithoutNameofOperatorAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Exceptions";
        private const string DiagnosticId = nameof(ArgumentExceptionWithoutNameofOperatorAnalyzer);
        private const string Message = "The field {0} is used in an ArgumentException as string. Consider using the nameof operator instead.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Suggest using the nameof operator in an ArgumentException.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = context.Node as ObjectCreationExpressionSyntax;
            if (objectCreationExpression == null)
            {
                return;
            }

            if (!objectCreationExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            var exceptionType = objectCreationExpression.Type;
            var symbolInformation = context.SemanticModel.GetSymbolInfo(exceptionType);
            if (symbolInformation.Symbol.InheritsFrom(typeof (ArgumentException)))
            {
                var arguments = objectCreationExpression.ArgumentList.Arguments.Select(x => x.Expression).OfType<LiteralExpressionSyntax>();
                var methodParameters = objectCreationExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.ParameterList.Parameters;

                // Exception is declared outside a method
                if (methodParameters == null)
                {
                    return;
                }

                foreach (var argument in arguments)
                {
                    var argumentName = argument.Token.Value;
                    var correspondingParameter = methodParameters.Value.FirstOrDefault(x => string.Equals((string) x.Identifier.Value, (string) argumentName, StringComparison.OrdinalIgnoreCase));
                    if (correspondingParameter != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, argument.GetLocation(), correspondingParameter.Identifier.Value));
                        return;
                    }
                }
            }
        }
    }
}