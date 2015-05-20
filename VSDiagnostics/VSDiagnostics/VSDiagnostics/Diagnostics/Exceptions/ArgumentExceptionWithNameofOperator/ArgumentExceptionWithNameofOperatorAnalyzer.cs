using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithNameofOperator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentExceptionWithNameofOperatorAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(ArgumentExceptionWithNameofOperatorAnalyzer);
        internal const string Title = "Suggest using the nameof operator in an ArgumentException.";
        internal const string Message = "The field {0} is used in an ArgumentException as string. Consider using the nameof operator instead.";
        internal const string Category = "Exceptions";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ThrowStatement);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var throwStatement = context.Node as ThrowStatementSyntax;
            if (throwStatement == null)
            {
                return;
            }

            var objectCreationExpression = throwStatement.Expression as ObjectCreationExpressionSyntax;
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
                var methodParameters = throwStatement.Ancestors().OfType<MethodDeclarationSyntax>().First().ParameterList.Parameters;

                foreach (var argument in arguments)
                {
                    var argumentName = argument.Token.Value;
                    var correspondingParameter = methodParameters.FirstOrDefault(x => string.Equals((string) x.Identifier.Value, (string) argumentName, StringComparison.OrdinalIgnoreCase));
                    if (correspondingParameter != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreationExpression.GetLocation(), correspondingParameter.Identifier.Value));
                        return;
                    }
                }
            }
        }
    }
}