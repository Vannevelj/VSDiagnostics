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
            if (surroundingInvocation != null && surroundingInvocation.IsNameofInvocation())
            {
                return;
            }

            // If we're dealing with a self-defined type 'Char' then we ignore it
            var identifierSymbol = context.SemanticModel.GetSymbolInfo(identifier);

            var typeSymbol = identifierSymbol.Symbol as INamedTypeSymbol;
            if (typeSymbol == null || typeSymbol.SpecialType == SpecialType.None)
            {
                return;
            }

            // If it is a qualified name like System.Char we have to include the namespace too
            // We don't need it in this step but we have to point the analyzer to the right location
            // This will make sure that we accept the entire qualified name in the code fix
            var location = identifier.GetLocation();
            var qualifiedName = identifier.AncestorsAndSelf().OfType<QualifiedNameSyntax>().FirstOrDefault();
            if (qualifiedName?.Parent is UsingDirectiveSyntax)
            {
                return;
            }

            if (qualifiedName != null)
            {
                location = qualifiedName.GetLocation();
            }

            // This ensures that we are not using aliases.  Both the identifier and the actual type must have a registered alias.
            // If the identifier alias and the alias for the actual type do not match (as when `using Single = System.String`),
            // the aliases do not match, so we do not create a diagnostic because the Simplifier does not create an alias in this case.
            string identifierAlias;
            string metadataAlias;
            if (identifier.Identifier.Text.HasAlias(out identifierAlias) && typeSymbol.MetadataName.HasAlias(out metadataAlias) && identifierAlias == metadataAlias)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, location, metadataAlias, typeSymbol.MetadataName));
            }
        }
    }
}