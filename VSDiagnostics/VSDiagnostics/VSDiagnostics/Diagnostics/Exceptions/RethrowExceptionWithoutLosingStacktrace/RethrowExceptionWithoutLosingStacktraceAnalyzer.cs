using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.RethrowExceptionWithoutLosingStacktrace
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RethrowExceptionWithoutLosingStacktraceAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;

        private static readonly string Message =
            VSDiagnosticsResources.RethrowExceptionWithoutLosingStacktraceAnalyzerMessage;

        private static readonly string Title =
            VSDiagnosticsResources.RethrowExceptionWithoutLosingStacktraceAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.RethrowExceptionWithoutLosingStacktrace, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ThrowStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var throwStatement = context.Node as ThrowStatementSyntax;

            var throwIdentifierSyntax = throwStatement?.Expression as IdentifierNameSyntax;
            if (throwIdentifierSyntax == null)
            {
                return;
            }

            var catchClause = throwStatement.Ancestors().OfType<CatchClauseSyntax>().FirstOrDefault();

            // Code is in an incomplete state (user is typing the catch clause but hasn't typed the identifier yet)
            var exceptionIdentifier = catchClause?.Declaration?.Identifier;
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