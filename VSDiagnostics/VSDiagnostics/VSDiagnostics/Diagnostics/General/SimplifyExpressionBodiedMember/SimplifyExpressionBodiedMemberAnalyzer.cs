using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyExpressionBodiedMemberAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(SimplifyExpressionBodiedMemberAnalyzer);
        internal const string Title = "Simplify the expression using an expression-bodied member.";
        internal const string Message = "{0} {1} can be written using an expression-bodied member";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
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

            var getter = propertyDeclaration.AccessorList.Accessors.FirstOrDefault(x => x.Keyword.ValueText == "get");
            if (getter == null)
            {
                return null;
            }

            if (getter.Body == null)
            {
                return null;
            }

            if (getter.Body.Statements.Count != 1)
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

            if (methodDeclaration.Body?.Statements.Count != 1)
            {
                return null;
            }

            var statement = methodDeclaration.Body.Statements.FirstOrDefault();
            if (statement == null)
            {
                return null;
            }

            var returnStatement = statement.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().FirstOrDefault();
            if (returnStatement == null)
            {
                return null;
            }

            return Diagnostic.Create(Rule, returnStatement.GetLocation(), "Method", methodDeclaration.Identifier.ValueText);
        }
    }
}