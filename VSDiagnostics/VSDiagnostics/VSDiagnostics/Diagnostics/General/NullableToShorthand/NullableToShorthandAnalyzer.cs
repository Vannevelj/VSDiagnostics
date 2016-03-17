using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NullableToShorthand
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableToShorthandAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.NullableToShorthandAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.NullableToShorthandAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.NullableToShorthand, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.GenericName);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var argumentList = (GenericNameSyntax) context.Node;
            if (argumentList.TypeArgumentList.Arguments.OfType<OmittedTypeArgumentSyntax>().Any())
            {
                return;
            }

            // Leave if type is in nullable form
            if (argumentList.IsKind(SyntaxKind.NullableType))
            {
                return;
            }

            var genericType = context.SemanticModel.GetSymbolInfo(argumentList);
            if (genericType.Symbol?.MetadataName != "Nullable`1")
            {
                return;
            }

            string identifier;
            // We're having a return type
            // We don't do this check together with the others to avoid interfering due to nested constructs (e.g. locals in a method)
            if (context.Node.Parent is MethodDeclarationSyntax)
            {
                identifier = "Return statement";
            }
            else
            {
                var variableAncestorNodes = new[]
                {
                    SyntaxKind.LocalDeclarationStatement,
                    SyntaxKind.FieldDeclaration,
                    SyntaxKind.Parameter,
                    SyntaxKind.TypeParameter,
                    SyntaxKind.PropertyDeclaration
                };

                // First we look through the different nodes that can't be nested
                // If nothing is found, we check if it's perhaps a standalone expression (such as an object creation without assigning it to an identifier)
                var parentNode =
                    context.Node.AncestorsAndSelf().FirstOrDefault(x => variableAncestorNodes.Contains(x.Kind())) ??
                    context.Node.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().FirstOrDefault();

                if (parentNode == null)
                {
                    return;
                }

                if (parentNode.Kind() == SyntaxKind.LocalDeclarationStatement)
                {
                    identifier = ((LocalDeclarationStatementSyntax) parentNode).Declaration?
                                                                               .Variables
                                                                               .FirstOrDefault()?
                                                                               .Identifier
                                                                               .Text;
                }
                else if (parentNode.Kind() == SyntaxKind.FieldDeclaration)
                {
                    identifier =
                        ((FieldDeclarationSyntax) parentNode).Declaration?
                                                             .Variables
                                                             .FirstOrDefault()?
                                                             .Identifier
                                                             .Text;
                }
                else if (parentNode.Kind() == SyntaxKind.Parameter)
                {
                    identifier = ((ParameterSyntax) parentNode).Identifier.Text;
                }
                else if (parentNode.Kind() == SyntaxKind.TypeParameter)
                {
                    identifier = ((TypeParameterSyntax) parentNode).Identifier.Text;
                }
                else if (parentNode.Kind() == SyntaxKind.PropertyDeclaration)
                {
                    identifier = ((PropertyDeclarationSyntax) parentNode).Identifier.Text;
                }
                else
                {
                    identifier = "Type declaration";
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), identifier));
        }
    }
}