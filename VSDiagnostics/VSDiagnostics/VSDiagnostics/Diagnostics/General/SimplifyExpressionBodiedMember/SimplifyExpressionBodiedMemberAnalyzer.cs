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
        private const string Category = "General";
        private const string DiagnosticId = nameof(SimplifyExpressionBodiedMemberAnalyzer);
        private const string Message = "{0} {1} can be written using an expression-bodied member";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Simplify the expression using an expression-bodied member.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

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

            if (propertyDeclaration.DescendantNodesAndTokensAndSelf().Any(x => x.GetLeadingTrivia().Concat(x.GetTrailingTrivia()).Any(y => !y.IsWhitespaceTrivia())))
            {
                return null;
            }

            var getter = propertyDeclaration.AccessorList.Accessors.FirstOrDefault(x => x.Keyword.IsKind(SyntaxKind.GetKeyword));
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

            var statement = getter.Body.Statements.First();
            return Diagnostic.Create(Rule, statement.GetLocation(), "Property", propertyDeclaration.Identifier);
        }

        private Diagnostic HandleMethod(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.ExpressionBody != null)
            {
                return null;
            }

            if (methodDeclaration.DescendantNodesAndTokensAndSelf().Any(x => x.GetLeadingTrivia().Concat(x.GetTrailingTrivia()).Any(y => !y.IsWhitespaceTrivia())))
            {
                return null;
            }

            if (methodDeclaration.Body?.Statements.Count != 1)
            {
                return null;
            }

            var statement = methodDeclaration.Body.Statements.FirstOrDefault();
            var returnStatement = statement?.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().FirstOrDefault();
            if (returnStatement == null)
            {
                return null;
            }

            return Diagnostic.Create(Rule, returnStatement.GetLocation(), "Method", methodDeclaration.Identifier.ValueText);
        }
    }
}