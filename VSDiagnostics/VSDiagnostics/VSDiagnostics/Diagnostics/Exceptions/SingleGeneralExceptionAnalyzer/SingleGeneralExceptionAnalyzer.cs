using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Exceptions.SingleGeneralExceptionAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingleGeneralExceptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SingleGeneralExceptionAnalyzer";
        internal const string Title = "Verifies whether a try-catch block does not contain just a single Exception clause.";
        internal const string Message = "A single catch-all clause has been used.";
        internal const string Category = "Exceptions";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.TryStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext obj)
        {
            var tryStatement = obj.Node as TryStatementSyntax;
            if (tryStatement == null)
            {
                return;
            }

            if (tryStatement.Catches.Count != 1)
            {
                return;
            }

            var catchClause = tryStatement.Catches.First();
            var declaredException = catchClause.Declaration?.Type;
            if (declaredException == null)
            {
                return;
            }

            var catchType = obj.SemanticModel.GetSymbolInfo(declaredException).Symbol.MetadataName;
            if (catchType == "Exception")
            {
                obj.ReportDiagnostic(Diagnostic.Create(Rule, catchClause.GetLocation()));
            }
        }
    }
}