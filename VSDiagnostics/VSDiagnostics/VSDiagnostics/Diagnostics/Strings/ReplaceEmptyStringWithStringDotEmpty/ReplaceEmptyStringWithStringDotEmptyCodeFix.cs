using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty
{
    [ExportCodeFixProvider("TestMethodWithoutPublicModifier", LanguageNames.CSharp), Shared]
    public class ReplaceEmptyStringWithStringDotEmptyCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ReplaceEmptyStringWithStringDotEmptyAnalyzer.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var literalDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
            context.RegisterCodeFix(CodeAction.Create("Use string.Empty", x => UseStringDotEmptyAsync(context.Document, root, literalDeclaration)), diagnostic);
        }

        private static Task<Solution> UseStringDotEmptyAsync(Document document, SyntaxNode root, LiteralExpressionSyntax literalDeclaration)
        {
            var stringDotEmptyInvocation = SyntaxFactory.ParseExpression("string.Empty");
            var newRoot = root.ReplaceNode(literalDeclaration, stringDotEmptyInvocation);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}