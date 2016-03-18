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
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.CastToAs
{
    [ExportCodeFixProvider(DiagnosticId.CastToAs + "CF", LanguageNames.CSharp), Shared]
    public class CastToAsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CastToAsAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.CastToAsCodeFixTitle,
                    x => CastToAsAsync(context.Document, root, statement), CastToAsAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> CastToAsAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var castExpression = (CastExpressionSyntax) statement;
            var newExpression =
                SyntaxFactory.BinaryExpression(SyntaxKind.AsExpression, castExpression.Expression, castExpression.Type)
                             .WithAdditionalAnnotations(Formatter.Annotation);

            var newRoot = root.ReplaceNode(castExpression, newExpression);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}