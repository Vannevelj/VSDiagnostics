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
            var arrowClause = SyntaxFactory.ArrowExpressionClause(expression);

            var property = statement.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (property != null)
            {
                root = root.ReplaceNode(property, property.RemoveNode(property.AccessorList, SyntaxRemoveOptions.KeepExteriorTrivia)
                                                          .WithExpressionBody(arrowClause)
                                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            var method = statement.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (method != null)
            {
                var trailingTrivia = method.Body.Statements.First().GetTrailingTrivia();
                root = root.ReplaceNode(method, method.RemoveNode(method.Body, SyntaxRemoveOptions.KeepExteriorTrivia)
                                                      .WithExpressionBody(arrowClause)
                                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                                      .WithTrailingTrivia(trailingTrivia));
            }

            var newDocument = document.WithSyntaxRoot(root);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}