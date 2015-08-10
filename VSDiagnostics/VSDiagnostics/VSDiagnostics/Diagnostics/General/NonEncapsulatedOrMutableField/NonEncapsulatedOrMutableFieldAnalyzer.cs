using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonEncapsulatedOrMutableFieldAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(NonEncapsulatedOrMutableFieldAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.NonEncapsulatedOrMutableFieldAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.NonEncapsulatedOrMutableFieldAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = context.Node as FieldDeclarationSyntax;
            if (fieldDeclaration == null)
            {
                return;
            }

            // Don't handle (semi-)immutable fields
            if (fieldDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.ConstKeyword) || x.IsKind(SyntaxKind.ReadOnlyKeyword)))
            {
                return;
            }

            // Only handle public, internal and protected internal fields
            if (!fieldDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword) || x.IsKind(SyntaxKind.InternalKeyword)))
            {
                return;
            }

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                // Using .Text instead of .ValueText so verbatim and unicode-escaped identifiers would display as such rather than having it stripped out.
                context.ReportDiagnostic(Diagnostic.Create(Rule, variable.Identifier.GetLocation(), variable.Identifier.Text));
            }
        }
    }
}