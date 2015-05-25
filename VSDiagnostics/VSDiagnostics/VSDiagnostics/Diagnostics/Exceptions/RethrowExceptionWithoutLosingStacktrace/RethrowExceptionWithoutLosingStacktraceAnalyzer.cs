using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Exceptions.RethrowExceptionWithoutLosingStacktrace
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RethrowExceptionWithoutLosingStacktraceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(RethrowExceptionWithoutLosingStacktraceAnalyzer);
        internal const string Title = "Warns when an exception is rethrown in a way that it loses the stacktrace.";
        internal const string Message = "Rethrown exception loses the stacktrace.";
        internal const string Category = "Exceptions";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ThrowStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var throwStatement = context.Node as ThrowStatementSyntax;
            if (throwStatement == null)
            {
                return;
            }

            var throwIdentifierSyntax = throwStatement.Expression as IdentifierNameSyntax;
            if (throwIdentifierSyntax == null)
            {
                return;
            }

            var catchClause = throwStatement.Ancestors().OfType<CatchClauseSyntax>().FirstOrDefault();
            if (catchClause == null)
            {
                return;
            }

            // Code is in an incomplete state (user is typing the catch clause but hasn't typed the identifier yet)
            var exceptionIdentifier = catchClause.Declaration?.Identifier;
            if (exceptionIdentifier == null)
            {
                return;
            }

            var catchClauseIdentifier = exceptionIdentifier.Value.ToString();
            var thrownIdentifier = throwIdentifierSyntax.Identifier.Value.ToString();

            if (string.Equals(catchClauseIdentifier, thrownIdentifier, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, throwStatement.GetLocation()));
            }
        }
    }
}