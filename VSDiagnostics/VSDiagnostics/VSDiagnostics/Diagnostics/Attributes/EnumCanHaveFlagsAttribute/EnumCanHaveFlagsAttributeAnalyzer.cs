using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumCanHaveFlagsAttributeAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.AttributesCategory;
        private static readonly string Message = VSDiagnosticsResources.EnumCanHaveFlagsAttributeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.EnumCanHaveFlagsAttributeAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.EnumCanHaveFlagsAttribute, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.EnumDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax) context.Node;

            foreach (var list in enumDeclaration.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
                    if (symbol == null || symbol.ContainingType.MetadataName == typeof (FlagsAttribute).Name)
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, enumDeclaration.GetLocation()));
        }
    }
}