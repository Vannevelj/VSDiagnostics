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
            var declarationExpression = (EnumDeclarationSyntax) context.Node;

            if (!declarationExpression.AttributeLists.Any(
                    a => a.Attributes.Any(
                        t => context.SemanticModel.GetSymbolInfo(t).Symbol.ContainingType.MetadataName == typeof(FlagsAttribute).Name)))
            {
                return;
            }

            var enumMemberDeclarations = declarationExpression.ChildNodes().OfType<EnumMemberDeclarationSyntax>().ToList();
            var values = enumMemberDeclarations.Select(member => member.EqualsValue);

            var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;

            foreach (var equalsValue in values)
            {
                // no value at all - "foo"
                if (equalsValue == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                    return;
                }

                LiteralExpressionSyntax valueExpression = null;

                // normal integer values - "foo = 4"
                if (equalsValue.Value is LiteralExpressionSyntax)
                {
                    valueExpression = (LiteralExpressionSyntax) equalsValue.Value;
                }

                // negative values - "foo = -4"
                // bitwise compliment values - "foo = ~4"
                // other prefix unary expressions except + - "foo = +4"
                if (equalsValue.Value is PrefixUnaryExpressionSyntax)
                {
                    var prefixUnaryExpression = (PrefixUnaryExpressionSyntax) equalsValue.Value;
                    if (!prefixUnaryExpression.OperatorToken.IsKind(SyntaxKind.PlusToken))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                        return;
                    }
                }

                // all other values
                if (valueExpression == null) { continue; }

                ulong value;

                if (!ulong.TryParse(valueExpression.Token.ValueText, out value) || !IsPowerOfTwo(value))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                    return;
                }
            }
        }

        private bool IsPowerOfTwo(ulong value)
        {
            var logValue = Math.Log(value, 2);
            return value == 0 || logValue - Math.Round(logValue) == 0;
        }
    }

    enum Test
    {
        Foo = 'r'
    }
}