﻿using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.Exceptions.SingleGeneralException
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingleGeneralExceptionAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Exceptions";
        private const string DiagnosticId = nameof(SingleGeneralExceptionAnalyzer);
        private const string Message = "A single catch-all clause has been used.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Verifies whether a try-catch block does not contain just a single Exception clause.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.TryStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var tryStatement = context.Node as TryStatementSyntax;
            if (tryStatement?.Catches.Count != 1)
            {
                return;
            }

            var catchClause = tryStatement.Catches.First();
            var declaredException = catchClause.Declaration?.Type;
            if (declaredException == null)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(declaredException).Symbol;
            if (symbol != null)
            {
                if (symbol.MetadataName == typeof(Exception).Name)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declaredException.GetLocation()));
                }
            }
        }
    }
}