using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.General.ConditionIsAlwaysFalse
{
    [ExportCodeFixProvider(nameof(ConditionIsAlwaysFalseCodeFix), LanguageNames.CSharp), Shared]
    public class ConditionIsAlwaysFalseCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ConditionIsAlwaysFalseAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.ConditionIsAlwaysTrueCodeFixTitle,
                    x => RemoveConditionAsync(context.Document, root, statement), ConditionIsAlwaysFalseAnalyzer.Rule.Id),
                diagnostic);
        }

        private Task<Solution> RemoveConditionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var ifStatement = statement.Ancestors().OfType<IfStatementSyntax>().First();
            var blockStatement = ifStatement.Else?.Statement as BlockSyntax;

            // no else
            var newRoot =
                root.RemoveNode(ifStatement, SyntaxRemoveOptions.KeepNoTrivia)
                    .WithAdditionalAnnotations(Formatter.Annotation);

            // else with braces
            if (blockStatement != null)
            {
                newRoot =
                    root.ReplaceNode(ifStatement, blockStatement.Statements)
                        .WithAdditionalAnnotations(Formatter.Annotation);
            }

            // else without braces
            if (ifStatement.Else != null && blockStatement == null)
            {
                newRoot =
                    root.ReplaceNode(ifStatement, ifStatement.Else.Statement)
                        .WithAdditionalAnnotations(Formatter.Annotation);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}