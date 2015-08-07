using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Add Modifier", x => AddModifier(context.Document, root, statement), nameof(ExplicitAccessModifiersAnalyzer)), diagnostic);
        }

        private Task<Solution> AddModifier(Document document, SyntaxNode root, SyntaxNode statement)
        {
            SyntaxNode newStatement = null;

            var classExpression = statement as ClassDeclarationSyntax;
            if (classExpression != null)
            {
                var internalKeywordToken = SyntaxFactory.Token(SyntaxKind.InternalKeyword);

                var newClass = classExpression.WithModifiers(classExpression.Modifiers.Add(internalKeywordToken));

                newStatement = statement.ReplaceNode(statement, newClass);
            }

            var newRoot = newStatement == null ? root : root.ReplaceNode(statement, newStatement);
            return Task.FromResult(document.WithSyntaxRoot(newRoot).Project.Solution);
        }
    }
}