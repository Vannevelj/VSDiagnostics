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

            var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;
            var enumMemberDeclarations = declarationExpression.ChildNodes().OfType<EnumMemberDeclarationSyntax>().ToList();

            foreach (var member in enumMemberDeclarations)
            {
                if (member.EqualsValue == null)
                {
                    continue;
                }

                var descendantNodes = member.EqualsValue.Value.DescendantNodesAndSelf().ToList();
                if (descendantNodes.OfType<LiteralExpressionSyntax>().Any() && descendantNodes.OfType<IdentifierNameSyntax>().Any())
                {
                    return;
                }
            }

            foreach (var member in enumMemberDeclarations)
            {
                // member doesn't have defined value - "foo" instead of "foo = 4"
                if (member.EqualsValue == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                    return;
                }

                if (member.EqualsValue.Value is BinaryExpressionSyntax)
                {
                    var descendantNodes = member.EqualsValue.Value.DescendantNodesAndSelf().ToList();
                    if (descendantNodes.Any() && descendantNodes.All(n => n is IdentifierNameSyntax || n is BinaryExpressionSyntax))
                    {
                        continue;
                    }
                }

                var symbol = context.SemanticModel.GetDeclaredSymbol(member);
                var value = symbol.ConstantValue;

                switch (value.GetType().Name)
                {
                    case nameof(Int16):
                        if (!IsPowerOfTwo((short)value))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                            return;
                        }
                        break;
                    case nameof(UInt16):
                        if (!IsPowerOfTwo((ushort)value))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                            return;
                        }
                        break;
                    case nameof(Int32):
                        if (!IsPowerOfTwo((int)value))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                            return;
                        }
                        break;
                    case nameof(UInt32):
                        if (!IsPowerOfTwo((uint)value))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                            return;
                        }
                        break;
                    case nameof(Int64):
                        if (!IsPowerOfTwo((long)value))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                            return;
                        }
                        break;
                    default:
                        if (!IsPowerOfTwo((ulong)value))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), enumName));
                            return;
                        }
                        break;
                }
            }
        }

        private bool IsPowerOfTwo(double value)
        {
            var logValue = Math.Log(value, 2);
            return value == 0 || logValue - Math.Round(logValue) == 0;
        }
    }
}