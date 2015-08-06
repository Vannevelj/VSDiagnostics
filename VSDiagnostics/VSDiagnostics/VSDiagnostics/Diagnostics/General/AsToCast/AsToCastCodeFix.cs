using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.General.AsToCast
{
    [ExportCodeFixProvider("SingleEmptyConstructorCodeFix", LanguageNames.CSharp), Shared]
    public class AsToCastCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AsToCastAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use explicit cast instead of as", x => RemoveConstructorAsync(context.Document, root, statement), nameof(AsToCastAnalyzer)), diagnostic);
        }

        private Task<Solution> RemoveConstructorAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var binaryExpression = (BinaryExpressionSyntax) statement;
            var typeSyntax = SyntaxFactory.ParseTypeName(binaryExpression.Right.GetText().ToString());
            var newExpression = SyntaxFactory.CastExpression(typeSyntax, binaryExpression.Left);

            var newRoot = root.ReplaceNode(binaryExpression, newExpression).WithAdditionalAnnotations(Formatter.Annotation);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}