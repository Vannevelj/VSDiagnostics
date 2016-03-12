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

namespace VSDiagnostics.Diagnostics.General.ConditionIsAlwaysTrue
{
    [ExportCodeFixProvider(nameof(ConditionIsAlwaysTrueCodeFix), LanguageNames.CSharp), Shared]
    public class ConditionIsAlwaysTrueCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ConditionIsAlwaysTrueAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.ConditionIsAlwaysTrueCodeFixTitle,
                    x => RemoveConditionAsync(context.Document, root, statement), ConditionIsAlwaysTrueAnalyzer.Rule.Id),
                diagnostic);
        }

        private Task<Solution> RemoveConditionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var ifStatement = statement.Ancestors().OfType<IfStatementSyntax>().First();

            var blockStatement = ifStatement.Statement as BlockSyntax;

            SyntaxNode newRoot;

            if (ifStatement.Parent.IsKind(SyntaxKind.ElseClause))
            {
                newRoot = blockStatement == null
                    ? root.ReplaceNode(ifStatement, ifStatement.Statement).WithAdditionalAnnotations(Formatter.Annotation)
                    : root.ReplaceNode(ifStatement, blockStatement).WithAdditionalAnnotations(Formatter.Annotation);
            }
            else
            {
                newRoot = blockStatement == null
                    ? root.ReplaceNode(ifStatement, ifStatement.Statement).WithAdditionalAnnotations(Formatter.Annotation)
                    : root.ReplaceNode(ifStatement, blockStatement.Statements).WithAdditionalAnnotations(Formatter.Annotation);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}