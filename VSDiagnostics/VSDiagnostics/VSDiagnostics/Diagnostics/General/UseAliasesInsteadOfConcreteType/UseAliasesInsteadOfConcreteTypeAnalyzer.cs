﻿using System.Collections.Immutable;
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

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.UseAliasesInsteadOfConcreteType, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.IdentifierName);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var identifier = context.Node as IdentifierNameSyntax;
            if (identifier == null)
            {
                return;
            }

            // A nameof() expression cannot contain aliases
            // There is no way to distinguish between a self-defined method 'nameof' and the nameof operator so we have to ignore all invocations that call into 'nameof'
            var surroundingInvocation = identifier.Ancestors().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            var invocationIdentifier = surroundingInvocation?.Expression.DescendantNodesAndSelf()
                                                             .OfType<IdentifierNameSyntax>()
                                                             .FirstOrDefault();

            if (invocationIdentifier != null && invocationIdentifier.Identifier.ValueText == "nameof")
            {
                return;
            }

            // If we're dealing with a self-defined type 'Char' then we ignore it
            var identifierSymbol = context.SemanticModel.GetSymbolInfo(identifier);
            if (identifierSymbol.Symbol == null)
            {
                return;
            }

            var typeSymbol = identifierSymbol.Symbol as INamedTypeSymbol;
            if (typeSymbol != null && typeSymbol.SpecialType == SpecialType.None)
            {
                return;
            }

            // If it is a qualified name like System.Char we have to include the namespace too
            // We don't need it in this step but we have to point the analyzer to the right location
            // This will make sure that we accept the entire qualified name in the code fix
            var location = identifier.GetLocation();
            var qualifiedName = identifier.AncestorsAndSelf().OfType<QualifiedNameSyntax>().FirstOrDefault();
            if (qualifiedName != null)
            {
                location = qualifiedName.GetLocation();
            }

            string alias;
            if (identifier.Identifier.Text.HasAlias(out alias))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, location, alias, identifier.Identifier.ValueText));
            }
        }
    }
}