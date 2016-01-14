using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Attributes.FlagsEnumValuesAreNotPowersOfTwo
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FlagsEnumValuesAreNotPowersOfTwoAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        private static readonly string Category = VSDiagnosticsResources.AttributesCategory;
        private static readonly string Message = VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoAnalyzerMessage;

        private static readonly string ValuesDontFitMessage =
            VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoValuesDontFitAnalyzerMessage;

        private static readonly string Title = VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoAnalyzerTitle;

        private static readonly string ValuesDontFitTitle =
            VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoValuesDontFitAnalyzerTitle;

        internal static DiagnosticDescriptor DefaultRule
            => new DiagnosticDescriptor(
                DiagnosticId.FlagsEnumValuesAreNotPowersOfTwo,
                Title,
                Message,
                Category,
                Severity,
                true);

        internal static DiagnosticDescriptor ValuesDontFitRule
            => new DiagnosticDescriptor(
                DiagnosticId.FlagsEnumValuesDontFit,
                ValuesDontFitTitle,
                ValuesDontFitMessage,
                Category, Severity,
                true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DefaultRule, ValuesDontFitRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.EnumDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (EnumDeclarationSyntax) context.Node;
            var flagsAttribute = declarationExpression.AttributeLists.FirstOrDefault(
                a => a.Attributes.FirstOrDefault(
                    t =>
                    {
                        var symbol = context.SemanticModel.GetSymbolInfo(t).Symbol;
                        return symbol == null || symbol.ContainingType.MetadataName == typeof(FlagsAttribute).Name;
                    }) != null);


            if (flagsAttribute == null)
            {
                return;
            }

            var enumName = context.SemanticModel.GetDeclaredSymbol(declarationExpression).Name;
            var enumMemberDeclarations =
                declarationExpression.ChildNodes().OfType<EnumMemberDeclarationSyntax>().ToList();

            foreach (var member in enumMemberDeclarations)
            {
                if (member.EqualsValue == null)
                {
                    continue;
                }

                var descendantNodes = member.EqualsValue.Value.DescendantNodesAndSelf().ToList();
                if (descendantNodes.OfType<LiteralExpressionSyntax>().Any() &&
                    descendantNodes.OfType<IdentifierNameSyntax>().Any())
                {
                    return;
                }
            }

            var enumType = declarationExpression.BaseList?.Types[0].Type;
            string keyword;
            if (enumType == null)
            {
                keyword = "int";
            }
            else
            {
                var typeSyntax = enumType as PredefinedTypeSyntax;
                if (typeSyntax != null)
                {
                    keyword = typeSyntax.Keyword.ValueText;
                }
                else
                {
                    var enumTypeInfo = context.SemanticModel.GetTypeInfo(enumType);
                    keyword = enumTypeInfo.Type.Name.ToAlias();
                }
            }

            // We have to make sure that by moving to powers of two, we won't exceed the type's maximum value 
            // For example: 255 is the last possible value for a byte enum
            if (IsOutsideOfRange(keyword, enumMemberDeclarations.Count))
            {
                context.ReportDiagnostic(Diagnostic.Create(ValuesDontFitRule,
                    declarationExpression.Identifier.GetLocation(),
                    enumName, keyword.ToLower()));
                return;
            }

            var createDiagnostic = false;
            foreach (var member in enumMemberDeclarations)
            {
                // member doesn't have defined value - "foo" instead of "foo = 4"
                if (member.EqualsValue == null)
                {
                    createDiagnostic = true;
                    continue;
                }

                if (member.EqualsValue.Value is BinaryExpressionSyntax)
                {
                    var descendantNodes = member.EqualsValue.Value.DescendantNodesAndSelf().ToList();
                    if (descendantNodes.Any() &&
                        descendantNodes.All(n => n is IdentifierNameSyntax || n is BinaryExpressionSyntax))
                    {
                        continue;
                    }
                }

                var symbol = context.SemanticModel.GetDeclaredSymbol(member);
                var value = symbol.ConstantValue;

                if (value == null) { return; }

                /* `value` is an `object`.  Casting it to `dynamic`
                 * will allow us to avoid a huge `switch` statement
                 * and allow us to just pass the value to `IsPowerOfTwo()`
                 */
                if (!IsPowerOfTwo((dynamic) value))
                {
                    createDiagnostic = true;
                }
            }

            if (createDiagnostic)
            {
                context.ReportDiagnostic(Diagnostic.Create(DefaultRule, declarationExpression.Identifier.GetLocation(),
                    enumName));
            }
        }

        /// <summary>
        ///     Determines whether a given value is a power of two
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns></returns>
        private bool IsPowerOfTwo(double value)
        {
            var logValue = Math.Log(value, 2);
            return Math.Abs(value) < 0.0001 || Math.Abs(logValue - Math.Round(logValue)) < 0.0001;
        }

        /// <summary>
        ///     Returns whether or not all values can be changed to powers of two without introducing out of range values.
        /// </summary>
        /// <param name="keyword">The type keyword that forms the base type of the enum</param>
        /// <param name="amountOfMembers">Indicates how many values an enum of this type can have</param>
        /// <returns></returns>
        private bool IsOutsideOfRange(string keyword, int amountOfMembers)
        {
            // The value represents the amount of members an enum of the given type can contain
            var rangeMapping = new Dictionary<string, int>
            {
                { "sbyte", 8 },
                { "byte", 9 },
                { "short", 16 },
                { "ushort", 17 },
                { "int", 32 },
                { "uint", 33 },
                { "long", 64 },
                { "ulong", 65 }
            };

            int amountAllowed;
            if (rangeMapping.TryGetValue(keyword, out amountAllowed))
            {
                return amountOfMembers > amountAllowed;
            }

            throw new ArgumentException("Unsupported base enum type encountered");
        }
    }
}