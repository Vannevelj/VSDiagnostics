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

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.PropertyDeclaration, SyntaxKind.MethodDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            Diagnostic diagnostic = null;
            
            if (context.Node.IsKind(SyntaxKind.PropertyDeclaration))
            {
                diagnostic = HandleProperty((PropertyDeclarationSyntax)context.Node);
            }
            
            if (context.Node.IsKind(SyntaxKind.MethodDeclaration))
            {
                diagnostic = HandleMethod((MethodDeclarationSyntax)context.Node);
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

            foreach (var declaration in propertyDeclaration.DescendantNodesAndTokensAndSelf())
            {
                foreach (var trivia in declaration.GetLeadingTrivia())
                {
                    if (!trivia.IsWhitespaceTrivia())
                    {
                        return null;
                    }
                }

                foreach (var trivia in declaration.GetTrailingTrivia())
                {
                    if (!trivia.IsWhitespaceTrivia())
                    {
                        return null;
                    }
                }
            }

            foreach (var accessor in propertyDeclaration.AccessorList.Accessors)
            {
                if (accessor.Keyword.IsKind(SyntaxKind.SetKeyword))
                {
                    return null;
                }
            }

            AccessorDeclarationSyntax getter = null;
            foreach (var accessor in propertyDeclaration.AccessorList.Accessors)
            {
                if (accessor.Keyword.IsKind(SyntaxKind.GetKeyword))
                {
                    getter = accessor;
                    break;
                }
            }
            if (getter == null)
            {
                return null;
            }

            foreach (var list in getter.AttributeLists)
            {
                if (list.Attributes.Any())
                {
                    return null;
                }
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

            foreach (var nodeOrToken in methodDeclaration.DescendantNodesAndTokensAndSelf())
            {
                if (nodeOrToken.GetLeadingTrivia().Concat(nodeOrToken.GetTrailingTrivia()).Any(y => !y.IsWhitespaceTrivia()))
                {
                    return null;
                }
            }

            if (methodDeclaration.Body?.Statements.Count != 1)
            {
                return null;
            }

            var statement = methodDeclaration.Body.Statements[0];
            if (!Nodes.Contains(statement.Kind()))
            {
                return null;
            }

            if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                var returnStatement = (ReturnStatementSyntax) statement;
                if (returnStatement.Expression == null)
                {
                    return null;
                }
            }

            return Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), "Method",
                methodDeclaration.Identifier.ValueText);
        }
    }
}