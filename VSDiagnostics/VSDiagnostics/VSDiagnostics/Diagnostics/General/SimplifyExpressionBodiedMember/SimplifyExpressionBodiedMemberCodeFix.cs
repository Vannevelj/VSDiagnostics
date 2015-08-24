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
    [ExportCodeFixProvider(nameof(SimplifyExpressionBodiedMemberCodeFix), LanguageNames.CSharp), Shared]
    public class SimplifyExpressionBodiedMemberCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SimplifyExpressionBodiedMemberAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.SimplifyExpressionBodiedMemberCodeFixTitle, x => UseExpressionBodiedMemberAsync(context.Document, root, statement),
                    nameof(SimplifyExpressionBodiedMemberAnalyzer)),
                diagnostic);
        }

        private Task<Solution> UseExpressionBodiedMemberAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var property = statement.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (property != null)
            {
                var firstStatement = property.AccessorList.Accessors.FirstOrDefault(x => x.Keyword.IsKind(SyntaxKind.GetKeyword)).Body.Statements.First();
                var arrowClause = SyntaxFactory.ArrowExpressionClause(((ReturnStatementSyntax) firstStatement).Expression);
                var newProperty = property.RemoveNode(property.AccessorList, SyntaxRemoveOptions.KeepNoTrivia)
                                          .WithExpressionBody(arrowClause)
                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));


                root = root.ReplaceNode(property, newProperty);
            }

            var method = statement.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (method != null)
            {
                var firstStatement = method.Body.Statements.First();
                var expression = firstStatement is ExpressionStatementSyntax ? ((ExpressionStatementSyntax) firstStatement).Expression : ((ReturnStatementSyntax) firstStatement).Expression;
                var arrowClause = SyntaxFactory.ArrowExpressionClause(expression);
                root = root.ReplaceNode(method, method.RemoveNode(method.Body, SyntaxRemoveOptions.KeepNoTrivia)
                                                      .WithExpressionBody(arrowClause)
                                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            return Task.FromResult(document.WithSyntaxRoot(root).Project.Solution);
        }
    }
}