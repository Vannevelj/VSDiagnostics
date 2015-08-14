using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumCanHaveFlagsAttributeAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(EnumCanHaveFlagsAttributeAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Info;

        private static readonly string Category = VSDiagnosticsResources.AttributesCategory;
        private static readonly string Message = VSDiagnosticsResources.EnumCanHaveFlagsAttributeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.EnumCanHaveFlagsAttributeAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.EnumDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            // enum must not already have flags attribute
            if (enumDeclaration.AttributeLists.Any(
                a => a.Attributes.Any(
                    t => {
                            var symbol = context.SemanticModel.GetSymbolInfo(t).Symbol;

                            return symbol == null || symbol.ContainingType.MetadataName == typeof(FlagsAttribute).Name;
                        })))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, enumDeclaration.GetLocation()));
        }
    }
}