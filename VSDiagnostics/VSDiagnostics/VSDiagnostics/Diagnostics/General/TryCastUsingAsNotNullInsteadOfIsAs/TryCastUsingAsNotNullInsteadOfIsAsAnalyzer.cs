using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.TryCastUsingAsNotNullInsteadOfIsAs
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TryCastUsingAsNotNullInsteadOfIsAsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(TryCastUsingAsNotNullInsteadOfIsAsAnalyzer);
        internal const string Title = "The conversion can be performed without casting twice.";
        internal const string Message = "Variable {0} can be casted using as/null.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.PropertyDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}