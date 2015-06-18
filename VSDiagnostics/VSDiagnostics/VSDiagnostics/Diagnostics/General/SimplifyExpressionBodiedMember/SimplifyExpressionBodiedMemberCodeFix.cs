using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember
{
    [ExportCodeFixProvider("SimplifyExpressionBodiedMember", LanguageNames.CSharp), Shared]
    public class SimplifyExpressionBodiedMemberCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use expression bodied member", x => UseExpressionBodiedMemberAsync(context.Document, root, statement)), diagnostic);
        }

        private Task<Solution> UseExpressionBodiedMemberAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var returnStatement = (ReturnStatementSyntax) statement;
            var expression = returnStatement.Expression;

            var property = statement.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (property != null)
            {
                var getter = property.AccessorList.Accessors.First();
                expression = expression.WithLeadingTrivia(
                    property.AccessorList.GetLeadingTrivia().Where(x => x.IsCommentTrivia()).Concat(
                        property.AccessorList.OpenBraceToken.TrailingTrivia.Where(x => x.IsCommentTrivia())));

                var arrowClause = SyntaxFactory.ArrowExpressionClause(expression)
                                               .WithTrailingTrivia(getter.GetTrailingTrivia().Where(x => x.IsCommentTrivia()));

                root = root.ReplaceNode(property, property.RemoveNode(property.AccessorList, SyntaxRemoveOptions.KeepNoTrivia)
                                                          .WithExpressionBody(arrowClause)
                                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                                                                           /*.WithTrailingTrivia(property.GetTrailingTrivia().Where(x => x.IsCommentTrivia()))*/));
            }

            var method = statement.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (method != null)
            {
                var body = method.Body.Statements.First();
                expression = expression
                    .WithLeadingTrivia(body.GetLeadingTrivia().Where(x => x.IsCommentTrivia()));

                var arrowClause = SyntaxFactory.ArrowExpressionClause(expression);

                root = root.ReplaceNode(method, method.RemoveNode(method.Body, SyntaxRemoveOptions.KeepNoTrivia)
                                                      .WithExpressionBody(arrowClause)
                                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                                      .WithTrailingTrivia(body.GetTrailingTrivia().Where(x => x.IsCommentTrivia()).Concat(
                                                          method.Body.CloseBraceToken.LeadingTrivia.Where(x => x.IsCommentTrivia()))));
            }

            var newDocument = document.WithSyntaxRoot(root);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}