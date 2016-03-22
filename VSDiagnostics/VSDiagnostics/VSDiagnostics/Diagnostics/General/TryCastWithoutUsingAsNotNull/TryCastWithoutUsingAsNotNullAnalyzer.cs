using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
// ReSharper disable LoopCanBeConvertedToQuery

namespace VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TryCastWithoutUsingAsNotNullAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.TryCastWithoutUsingAsNotNullAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.TryCastWithoutUsingAsNotNullAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.TryCastWithoutUsingAsNotNull, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IsExpression);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var isExpression = (BinaryExpressionSyntax) context.Node;

            var isIdentifierExpression = isExpression.Left as IdentifierNameSyntax;
            if (isIdentifierExpression == null)
            {
                return;
            }
            var isIdentifier = isIdentifierExpression.Identifier.ValueText;
            var isType = context.SemanticModel.GetTypeInfo(isExpression.Right).Type;
            if (isType == null)
            {
                return;
            }

            var ifStatement = isExpression.AncestorsAndSelf().NonLinqOfType<IfStatementSyntax>(SyntaxKind.IfStatement).NonLinqFirstOrDefault();
            if (ifStatement == null)
            {
                return;
            }

            var asExpressions = new List<BinaryExpressionSyntax>();

            foreach (var node in ifStatement.Statement.DescendantNodes())
            {
                var expression = node as BinaryExpressionSyntax;
                if (expression != null && expression.OperatorToken.IsKind(SyntaxKind.AsKeyword))
                {
                    asExpressions.Add(expression);
                }
            }

            foreach (var node in ifStatement.Condition.DescendantNodesAndSelf())
            {
                var expression = node as BinaryExpressionSyntax;
                if (expression != null && expression.OperatorToken.IsKind(SyntaxKind.AsKeyword))
                {
                    asExpressions.Add(expression);
                }
            }

            var castExpressions = new List<CastExpressionSyntax>();
            castExpressions.AddRange(ifStatement.Statement.DescendantNodes().NonLinqOfType<CastExpressionSyntax>(SyntaxKind.CastExpression));
            castExpressions.AddRange(ifStatement.Condition.DescendantNodesAndSelf().NonLinqOfType<CastExpressionSyntax>(SyntaxKind.CastExpression));

            Action reportDiagnostic = () => context.ReportDiagnostic(Diagnostic.Create(Rule, isExpression.GetLocation(), isIdentifier));

            Action<string, TypeInfo> checkRequirements = (bodyIdentifier, castedType) =>
            {
                if (castedType.Type == null)
                {
                    return;
                }

                if (bodyIdentifier == isIdentifier)
                {
                    // If the cast is of type Nullable<T> then we have to look at the generic argument
                    // A direct cast and an 'is' operator will use 'int' but the corresponding 'as' cast uses 'Nullable<int>'
                    if (castedType.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    {
                        var nullableType = castedType.Type as INamedTypeSymbol;
                        var argument = nullableType?.TypeArguments.NonLinqFirstOrDefault();
                        if (argument == null)
                        {
                            return;
                        }

                        if (argument.Equals(isType))
                        {
                            reportDiagnostic();
                        }
                    }
                    else if (castedType.Type.Equals(isType))
                    {
                        reportDiagnostic();
                    }
                }
            };

            var expressions = new List<ExpressionSyntax>(asExpressions);
            expressions.AddRange(castExpressions);

            foreach (var expression in expressions)
            {
                var binaryExpression = expression as BinaryExpressionSyntax;
                var binaryIdentifier = binaryExpression?.Left as IdentifierNameSyntax;
                if (binaryIdentifier != null)
                {
                    var castedType = context.SemanticModel.GetTypeInfo(binaryExpression.Right);
                    checkRequirements(binaryIdentifier.Identifier.ValueText, castedType);
                    continue;
                }

                var castExpression = expression as CastExpressionSyntax;
                var castIdentifier = castExpression?.Expression as IdentifierNameSyntax;
                if (castIdentifier != null)
                {
                    var castedType = context.SemanticModel.GetTypeInfo(castExpression.Type);
                    checkRequirements(castIdentifier.Identifier.ValueText, castedType);
                }
            }
        }
    }
}