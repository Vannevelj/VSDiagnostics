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

namespace VSDiagnostics.Diagnostics.General.NullableToShorthand
{
    [ExportCodeFixProvider("NullableToShorthand", LanguageNames.CSharp), Shared]
    public class NullableToShorthandCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(NullableToShorthandAnalyzer.DiagnosticId);
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start);
            context.RegisterFix(CodeAction.Create("Use shorthand notation", x => UseShorthandNotation(context.Document, root, declaration)), diagnostic);
        }

        private static async Task<Solution> UseShorthandNotation(Document document, SyntaxNode root, SyntaxToken declaration)
        {
            var node = root.FindNode(declaration.Span);
            var typeNode = (GenericNameSyntax) node;
            var semanticModel = await document.GetSemanticModelAsync();
            var type = semanticModel.GetTypeInfo(typeNode.TypeArgumentList.Arguments.First()).Type;

            var newExpression = SyntaxFactory.ParseTypeName(type.MetadataName + "?").WithLeadingTrivia(node.GetLeadingTrivia()).WithTrailingTrivia(SyntaxFactory.Space);

            var newParent = node.ReplaceNode(node, newExpression);
            var newRoot = root.ReplaceNode(node, newParent);
            var formattedRoot = Formatter.Format(newRoot, newParent.Span, document.Project.Solution.Workspace, document.Project.Solution.Workspace.Options);
            var newDocument = document.WithSyntaxRoot(formattedRoot);

            return newDocument.Project.Solution;
        }
    }
}