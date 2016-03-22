﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
// ReSharper disable LoopCanBeConvertedToQuery

namespace VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceEmptyStringWithStringDotEmptyAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.StringsCategory;

        private static readonly string Message =
            VSDiagnosticsResources.ReplaceEmptyStringWithStringDotEmptyAnalyzerMessage;

        private static readonly string Title = VSDiagnosticsResources.ReplaceEmptyStringWithStringDotEmptyAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ReplaceEmptyStringWithStringDotEmpty, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StringLiteralExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.AncestorsAndSelf().NonLinqOfType<AttributeArgumentSyntax>(SyntaxKind.AttributeArgument).NonLinqAny())
            {
                return;
            }

            var stringLiteral = (LiteralExpressionSyntax) context.Node;

            if (stringLiteral.Token.Text != "\"\"")
            {
                return;
            }

            foreach (var node in stringLiteral.Ancestors())
            {
                if (node.IsKind(SyntaxKind.Parameter))
                {
                    return;
                }
            }

            var variableDeclaration = stringLiteral.Ancestors().NonLinqOfType<FieldDeclarationSyntax>(SyntaxKind.FieldDeclaration).NonLinqFirstOrDefault();
            if (variableDeclaration != null)
            {
                foreach (var modifier in variableDeclaration.Modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.ConstKeyword))
                    {
                        return;
                    }
                }
            }

            // A switch label in the scenario of 
            // switch(var)
            // {
            //     case "": break;
            // }
            // Cannot be changed since it has to be a constant
            foreach (var label in stringLiteral.AncestorsAndSelf())
            {
                if (label is SwitchLabelSyntax)
                {
                    return;
                }
            }
            /*{
                return;
            }*/

            context.ReportDiagnostic(Diagnostic.Create(Rule, stringLiteral.GetLocation()));
        }
    }
}