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

            var conversionOperatorDeclarationSyntax = context.Node as ConversionOperatorDeclarationSyntax;
            if (conversionOperatorDeclarationSyntax != null)
            {
                var expression = conversionOperatorDeclarationSyntax;
                typeExpression = expression.Type;
            }

            var delegateDeclarationSyntax = context.Node as DelegateDeclarationSyntax;
            if (delegateDeclarationSyntax != null)
            {
                var expression = delegateDeclarationSyntax;
                typeExpression = expression.ReturnType;
            }

            var indexerDeclarationSyntax = context.Node as IndexerDeclarationSyntax;
            if (indexerDeclarationSyntax != null)
            {
                var expression = indexerDeclarationSyntax;
                typeExpression = expression.Type;
            }

            var methodDeclarationSyntax = context.Node as MethodDeclarationSyntax;
            if (methodDeclarationSyntax != null)
            {
                var expression = methodDeclarationSyntax;
                typeExpression = expression.ReturnType;
            }

            var operatorDeclarationSyntax = context.Node as OperatorDeclarationSyntax;
            if (operatorDeclarationSyntax != null)
            {
                var expression = operatorDeclarationSyntax;
                typeExpression = expression.ReturnType;
            }

            var propertyDeclarationSyntax = context.Node as PropertyDeclarationSyntax;
            if (propertyDeclarationSyntax != null)
            {
                var expression = propertyDeclarationSyntax;
                typeExpression = expression.Type;
            }

            var declarationSyntax = context.Node as VariableDeclarationSyntax;
            if (declarationSyntax != null)
            {
                var expression = declarationSyntax;
                typeExpression = expression.Type;
            }

            var expressionSyntax = context.Node as MemberAccessExpressionSyntax;
            if (expressionSyntax != null)
            {
                var expression = expressionSyntax;
                typeExpression = expression.Expression as IdentifierNameSyntax;
            }

            var syntax = context.Node as ParameterSyntax;
            if (syntax != null)
            {
                var expression = syntax;
                typeExpression = expression.Type;
            }

            var node = context.Node as TypeOfExpressionSyntax;
            if (node != null)
            {
                var expression = node;
                typeExpression = expression.Type;
            }

            var listSyntax = context.Node as TypeArgumentListSyntax;
            if (listSyntax != null)
            {
                var expression = listSyntax;
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

            if (typeName.IsAlias())
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, typeExpression.GetLocation(), typeName.ToAlias(), typeName));
            }
        }
    }
}