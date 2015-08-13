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

            var enunMemberDeclarations = declarationExpression.ChildNodes().OfType<EnumMemberDeclarationSyntax>().ToList();
            var values = enunMemberDeclarations.Select(member => member.EqualsValue);

            foreach (var equalsValue in values)
            {
                if (equalsValue == null)
                {
                    var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                    return;
                }

                LiteralExpressionSyntax valueExpression = null;

                if (equalsValue.Value is LiteralExpressionSyntax)
                {
                    valueExpression = (LiteralExpressionSyntax) equalsValue.Value;
                }
                if (equalsValue.Value is PrefixUnaryExpressionSyntax)
                {
                    var prefixUnaryExpression = (PrefixUnaryExpressionSyntax) equalsValue.Value;
                    if (prefixUnaryExpression.OperatorToken.IsKind(SyntaxKind.MinusToken))
                    {
                        var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;
                        context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                        return;
                    }
                }

                if (valueExpression == null) { continue; }

                ulong value;

                if (!ulong.TryParse(valueExpression.Token.ValueText, out value) || !IsPowerOfTwo(value))
                {
                    var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;
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
}