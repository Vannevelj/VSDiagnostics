using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyExpressionBodiedMemberAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.SimplifyExpressionBodiedMemberAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.SimplifyExpressionBodiedMemberAnalyzerTitle;

        private static readonly List<SyntaxKind> Nodes = new List<SyntaxKind>();

        static SimplifyExpressionBodiedMemberAnalyzer()
        {
            Nodes.AddRange(new[]
            {
                SyntaxKind.ExpressionStatement,
                SyntaxKind.ReturnStatement
            });
        }

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.SimplifyExpressionBodiedMember, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.PropertyDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            Diagnostic diagnostic = null;

            var asProperty = context.Node as PropertyDeclarationSyntax;
            if (asProperty != null)
            {
                diagnostic = HandleProperty(asProperty);
            }

            var asMethod = context.Node as MethodDeclarationSyntax;
            if (asMethod != null)
            {
                diagnostic = HandleMethod(asMethod);
            }

            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic HandleProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.ExpressionBody != null)
            {
                return null;
            }

            if (
                propertyDeclaration.DescendantNodesAndTokensAndSelf()
                    .Any(x => x.GetLeadingTrivia().Concat(x.GetTrailingTrivia()).Any(y => !y.IsWhitespaceTrivia())))
            {
                return null;
            }

            if (propertyDeclaration.AccessorList.Accessors.Any(x => x.Keyword.IsKind(SyntaxKind.SetKeyword)))
            {
                return null;
            }

            var getter =
                propertyDeclaration.AccessorList.Accessors.FirstOrDefault(x => x.Keyword.IsKind(SyntaxKind.GetKeyword));
            if (getter == null)
            {
                return null;
            }

            if (getter.AttributeLists.Any(x => x.Attributes.Any()))
            {
                return null;
            }

            if (getter.Body?.Statements.Count != 1)
            {
                return null;
            }

            if (!Nodes.Contains(getter.Body.Statements[0].Kind()))
            {
                return null;
            }

            var statement = getter.Body.Statements.First();
            return Diagnostic.Create(Rule, statement.GetLocation(), "Property", propertyDeclaration.Identifier);
        }

        private Diagnostic HandleMethod(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.ExpressionBody != null)
            {
                return null;
            }

            if (
                methodDeclaration.DescendantNodesAndTokensAndSelf()
                    .Any(x => x.GetLeadingTrivia().Concat(x.GetTrailingTrivia()).Any(y => !y.IsWhitespaceTrivia())))
            {
                return null;
            }

            if (methodDeclaration.Body?.Statements.Count != 1)
            {
                return null;
            }

            if (!Nodes.Contains(methodDeclaration.Body.Statements[0].Kind()))
            {
                return null;
            }

            return Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), "Method",
                methodDeclaration.Identifier.ValueText);
        }
    }
}