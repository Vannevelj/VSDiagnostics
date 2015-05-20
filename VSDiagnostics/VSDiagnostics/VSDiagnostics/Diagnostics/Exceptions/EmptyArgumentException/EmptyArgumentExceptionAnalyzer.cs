using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.EmptyArgumentException
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyArgumentExceptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(EmptyArgumentExceptionAnalyzer);
        internal const string Title = "Verifies whether an ArgumentException is thrown with a message.";
        internal const string Message = "ArgumentException is thrown without a message.";
        internal const string Category = "Exceptions";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ThrowStatement);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var throwStatement = context.Node as ThrowStatementSyntax;
            if (throwStatement == null)
            {
                return;
            }

            var expression = throwStatement.Expression as ObjectCreationExpressionSyntax;
            if (expression == null)
            {
                return;
            }

            var symbolInformation = context.SemanticModel.GetSymbolInfo(expression.Type);
            if (!symbolInformation.Symbol.InheritsFrom(typeof (ArgumentException)))
            {
                return;
            }

            if (!expression.ArgumentList.ChildNodes().Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, expression.GetLocation()));
            }
        }
    }
}