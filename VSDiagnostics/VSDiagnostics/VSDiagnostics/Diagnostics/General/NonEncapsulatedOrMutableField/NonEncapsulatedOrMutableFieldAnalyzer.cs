using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonEncapsulatedOrMutableFieldAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(NonEncapsulatedOrMutableFieldAnalyzer);
        internal const string Title = "Internal or public fields should be immutable or a property.";
        internal const string Message = "Field {0} should be turned into a property.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}