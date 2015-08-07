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

namespace VSDiagnostics.Diagnostics.General.ExplicitAccessModifiers
{
    [ExportCodeFixProvider("ExplicitAccessModifiers", LanguageNames.CSharp), Shared]
    public class ExplicitAccessModifiersCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ExplicitAccessModifiersAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var identifier = root.FindToken(diagnosticSpan.Start);
            context.RegisterCodeFix(CodeAction.Create("Add Modifier", x => AddModifier(context.Document, root, identifier), nameof(ExplicitAccessModifiersAnalyzer)), diagnostic);
        }

        private Task<Solution> AddModifier(Document document, SyntaxNode root, SyntaxToken identifier)
        {
            var identifierParent = identifier.Parent;
            SyntaxNode newParent = null;

            var classExpression = identifierParent as ClassDeclarationSyntax;
            if (classExpression != null && !classExpression.Modifiers.Any())
            {
                var token = SyntaxFactory.Token(SyntaxKind.InternalKeyword);
                classExpression.Modifiers.Add(token);

                newParent = identifierParent.ReplaceNode(identifierParent, classExpression);
            }

            var newRoot = newParent == null ? root : root.ReplaceNode(identifierParent, newParent);
            return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
        }
    }
}