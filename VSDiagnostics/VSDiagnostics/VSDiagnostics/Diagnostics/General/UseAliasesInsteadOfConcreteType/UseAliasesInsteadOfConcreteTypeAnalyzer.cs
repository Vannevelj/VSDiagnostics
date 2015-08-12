using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.UseAliasesInsteadOfConcreteType
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseAliasesInsteadOfConcreteTypeAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = nameof(UseAliasesInsteadOfConcreteTypeAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var expression = context.Node as VariableDeclarationSyntax;

            if (!(expression?.Type is IdentifierNameSyntax))
            {
                return;
            }

            var typeExpression = (IdentifierNameSyntax) expression.Type;

            if (typeExpression.IsVar)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(typeExpression).Symbol;
            var typeName = symbol.MetadataName;

            if (symbol.ContainingNamespace.Name != "System")
            {
                return;
            }

            if (MapConcreteTypeToPredefinedTypeAlias.ContainsKey(typeName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, typeExpression.GetLocation(),
                    MapConcreteTypeToPredefinedTypeAlias.First(kvp => kvp.Key == typeName).Value, typeName));
            }
        }

        public static readonly Dictionary<string, string> MapConcreteTypeToPredefinedTypeAlias =
            new Dictionary<string, string>
            {
                {"Int16", "short"},
                {"Int32", "int"},
                {"Int64", "long"},
                {"UInt16", "ushort"},
                {"UInt32", "uint"},
                {"UInt64", "ulong"},
                {"Object", "object"},
                {"Byte", "byte"},
                {"SByte", "sbyte"},
                {"Char", "char"},
                {"Boolean", "bool"},
                {"Single", "float"},
                {"Double", "double"},
                {"Decimal", "decimal"},
                {"String", "string"}
            };
    }
}