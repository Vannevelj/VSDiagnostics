using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember
{
    [ExportCodeFixProvider("TypeToVar", LanguageNames.CSharp), Shared]
    public class SimplifyExpressionBodiedMemberCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use expression bodied member", x => UseExpressionBodiedMember(context.Document, root, statement)), diagnostic);
        }

        private Task<Solution> UseExpressionBodiedMember(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var parentProperty = statement.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (parentProperty != null)
            {
                root = root.ReplaceNode(parentProperty, HandleProperty(parentProperty, statement));
            }

            var parentMethod = statement.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (parentMethod != null)
            {
                root = root.ReplaceNode(parentMethod, HandleMethod(parentMethod, statement));
            }

            var newDocument = document.WithSyntaxRoot(root);
            return Task.FromResult(newDocument.Project.Solution);
        }

        private SyntaxNode HandleProperty(PropertyDeclarationSyntax property, SyntaxNode statement)
        {
            var returnStatement = (ReturnStatementSyntax) statement;
            var expression = returnStatement.Expression;
            var arrowClause = SyntaxFactory.ArrowExpressionClause(expression);
            return property.RemoveNode(property.AccessorList, SyntaxRemoveOptions.KeepNoTrivia)
                           .WithExpressionBody(arrowClause)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private SyntaxNode HandleMethod(MethodDeclarationSyntax method, SyntaxNode statement)
        {
            var returnStatement = (ReturnStatementSyntax) statement;
            var expression = returnStatement.Expression;
            var arrowClause = SyntaxFactory.ArrowExpressionClause(expression);
            return method.RemoveNode(method.Body, SyntaxRemoveOptions.KeepNoTrivia)
                         .WithExpressionBody(arrowClause)
                         .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
    }
}