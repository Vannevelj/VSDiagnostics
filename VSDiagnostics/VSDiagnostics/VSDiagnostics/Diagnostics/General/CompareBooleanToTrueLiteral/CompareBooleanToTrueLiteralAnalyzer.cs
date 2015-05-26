using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompareBooleanToTrueLiteralAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(CompareBooleanToTrueLiteralAnalyzer);
        internal const string Title = "A boolean expression doesn't have to be compared to true.";
        internal const string Message = "A boolean expression can be simplified.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.TrueLiteralExpression);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}