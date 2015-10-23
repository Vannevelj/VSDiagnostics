using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.UseAliasesInsteadOfConcreteType
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseAliasesInsteadOfConcreteTypeAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeAnalyzerTitle;

        private static readonly Dictionary<string, string> MapConcreteTypeToPredefinedTypeAlias =
            new Dictionary<string, string>
            {
                {nameof(Int16), "short"},
                {nameof(Int32), "int"},
                {nameof(Int64), "long"},
                {nameof(UInt16), "ushort"},
                {nameof(UInt32), "uint"},
                {nameof(UInt64), "ulong"},
                {nameof(Object), "object"},
                {nameof(Byte), "byte"},
                {nameof(SByte), "sbyte"},
                {nameof(Char), "char"},
                {nameof(Boolean), "bool"},
                {nameof(Single), "float"},
                {nameof(Double), "double"},
                {nameof(Decimal), "decimal"},
                {nameof(String), "string"}
            };

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.UseAliasesInsteadOfConcreteType, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol,
                SyntaxKind.ConversionOperatorDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.VariableDeclaration,
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.Parameter,
                SyntaxKind.TypeOfExpression,
                SyntaxKind.TypeArgumentList);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            TypeSyntax typeExpression = null;

            if (context.Node is ConversionOperatorDeclarationSyntax)
            {
                var expression = (ConversionOperatorDeclarationSyntax) context.Node;
                typeExpression = expression.Type;
            }

            if (context.Node is DelegateDeclarationSyntax)
            {
                var expression = (DelegateDeclarationSyntax) context.Node;
                typeExpression = expression.ReturnType;
            }

            if (context.Node is IndexerDeclarationSyntax)
            {
                var expression = (IndexerDeclarationSyntax) context.Node;
                typeExpression = expression.Type;
            }

            if (context.Node is MethodDeclarationSyntax)
            {
                var expression = (MethodDeclarationSyntax) context.Node;
                typeExpression = expression.ReturnType;
            }

            if (context.Node is OperatorDeclarationSyntax)
            {
                var expression = (OperatorDeclarationSyntax) context.Node;
                typeExpression = expression.ReturnType;
            }

            if (context.Node is PropertyDeclarationSyntax)
            {
                var expression = (PropertyDeclarationSyntax) context.Node;
                typeExpression = expression.Type;
            }

            if (context.Node is VariableDeclarationSyntax)
            {
                var expression = (VariableDeclarationSyntax) context.Node;
                typeExpression = expression.Type;
            }

            if (context.Node is MemberAccessExpressionSyntax)
            {
                var expression = (MemberAccessExpressionSyntax) context.Node;
                typeExpression = expression.Expression as IdentifierNameSyntax;
            }

            if (context.Node is ParameterSyntax)
            {
                var expression = (ParameterSyntax) context.Node;
                typeExpression = expression.Type;
            }

            if (context.Node is TypeOfExpressionSyntax)
            {
                var expression = (TypeOfExpressionSyntax) context.Node;
                typeExpression = expression.Type;
            }

            if (context.Node is TypeArgumentListSyntax)
            {
                var expression = (TypeArgumentListSyntax) context.Node;
                foreach (var argument in expression.Arguments)
                {
                    typeExpression = argument;
                }
            }

            if (typeExpression == null ||
                !(typeExpression is IdentifierNameSyntax) &&
                !(typeExpression is QualifiedNameSyntax))
            {
                return;
            }

            if (typeExpression.IsVar)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(typeExpression).Symbol;
            if (symbol == null)
            {
                return;
            }

            var typeName = symbol.MetadataName;
            var namespaceName = symbol.ContainingNamespace.Name;

            if (namespaceName != "System")
            {
                return;
            }

            if (MapConcreteTypeToPredefinedTypeAlias.ContainsKey(typeName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, typeExpression.GetLocation(),
                    MapConcreteTypeToPredefinedTypeAlias.First(kvp => kvp.Key == typeName).Value, typeName));
            }
        }
    }
}