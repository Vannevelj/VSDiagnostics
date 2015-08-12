using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.FlagsEnumValuesAreNotPowersOfTwo
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FlagsEnumValuesAreNotPowersOfTwoAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(FlagsEnumValuesAreNotPowersOfTwoAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.EnumDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = context.Node as EnumDeclarationSyntax;
            if (declarationExpression == null)
            {
                return;
            }

            var enunMemberDeclarations = declarationExpression.ChildNodes().OfType<EnumMemberDeclarationSyntax>();

            if (enunMemberDeclarations.Select(member => member.EqualsValue.Value as LiteralExpressionSyntax).Any(valueToken => !IsPowerOfTwo(int.Parse(valueToken.Token.ValueText))))
            {
                var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
            }
        }

        private bool IsPowerOfTwo(int value)
        {
            var valueAsBinary = Convert.ToString(value, 2);
            return valueAsBinary == "0" || valueAsBinary.LastIndexOf('1') == 0;
        }
    }
}