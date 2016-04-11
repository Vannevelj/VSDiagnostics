using System.Collections.Immutable;
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
            if (argumentList.TypeArgumentList.Arguments.SyntaxNodeOfType<OmittedTypeArgumentSyntax>(SyntaxKind.OmittedTypeArgument).NonLinqAny())
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
                SyntaxNode parentNode = null;
                foreach (var node in context.Node.AncestorsAndSelf())
                {
                    if (variableAncestorNodes.NonLinqContains(node.Kind()))
                    {
                        parentNode = node;
                    }
                }

                if (parentNode == null)
                {
                    parentNode = context.Node.AncestorsAndSelf().SyntaxNodeOfType<ExpressionStatementSyntax>(SyntaxKind.ExpressionStatement).NonLinqFirstOrDefault();

                    if (parentNode == null)
                    {
                        return;
                    }
                }

                if (parentNode.Kind() == SyntaxKind.LocalDeclarationStatement)
                {
                    // ReSharper disable once PossibleInvalidCastException
                    identifier = ((LocalDeclarationStatementSyntax) parentNode).Declaration?
                                                                               .Variables
                                                                               .FirstOrDefault()?
                                                                               .Identifier
                                                                               .Text;
                }
                else if (parentNode.Kind() == SyntaxKind.FieldDeclaration)
                {
                    // ReSharper disable once PossibleInvalidCastException
                    identifier = ((FieldDeclarationSyntax) parentNode).Declaration?
                                                                      .Variables
                                                                      .FirstOrDefault()?
                                                                      .Identifier
                                                                      .Text;
                }
                else if (parentNode.Kind() == SyntaxKind.Parameter)
                {
                    // ReSharper disable once PossibleInvalidCastException
                    identifier = ((ParameterSyntax) parentNode).Identifier.Text;
                }
                else if (parentNode.Kind() == SyntaxKind.TypeParameter)
                {
                    // ReSharper disable once PossibleInvalidCastException
                    identifier = ((TypeParameterSyntax) parentNode).Identifier.Text;
                }
                else if (parentNode.Kind() == SyntaxKind.PropertyDeclaration)
                {
                    // ReSharper disable once PossibleInvalidCastException
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