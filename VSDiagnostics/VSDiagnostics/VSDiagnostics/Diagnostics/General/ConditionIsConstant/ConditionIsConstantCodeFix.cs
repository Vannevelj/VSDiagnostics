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

namespace VSDiagnostics.Diagnostics.General.ConditionIsConstant
{
    [ExportCodeFixProvider(nameof(ConditionIsConstantCodeFix), LanguageNames.CSharp), Shared]
    public class ConditionIsConstantCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(General.ConditionIsConstant.ConditionIsConstantAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);

            if (bool.Parse(diagnostic.Properties["IsConditionTrue"]))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(VSDiagnosticsResources.ConditionIsConstantCodeFixTitle,
                        x => RemoveConstantTrueConditionAsync(context.Document, root, statement),
                        ConditionIsConstantAnalyzer.Rule.Id),
                    diagnostic);
            }
            else
            {
                context.RegisterCodeFix(
                    CodeAction.Create(VSDiagnosticsResources.ConditionIsConstantCodeFixTitle,
                        x => RemoveConstantFalseConditionAsync(context.Document, root, statement),
                        ConditionIsConstantAnalyzer.Rule.Id),
                    diagnostic);
            }
        }

        private Task<Solution> RemoveConstantTrueConditionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var ifStatement = statement.Ancestors().OfType<IfStatementSyntax>().First();

            var blockStatement = ifStatement.Statement as BlockSyntax;

            SyntaxNode newRoot;

            /* this condition will be true when the `if` does not have braces:

               if (condition) statement;
            */
            if (blockStatement == null)
            {
                newRoot = root.ReplaceNode(ifStatement, ifStatement.Statement).WithAdditionalAnnotations(Formatter.Annotation);
            }
            else
            {
                /* if the if statement's parent is `SyntaxKind.ElseClause`,
                   the `else` does not have braces and needs the entire `if` block, braces and all:

                   else if (condition) { statement; }

                   otherwise, the block is already there and we need to replace `if` with just the block statements
                   also covers the general `if`-with-braces condition

                   else { if (condition) { statements; } }
                   if (condition) { statements; }
                */
                newRoot = ifStatement.Parent.IsKind(SyntaxKind.ElseClause)
                    ? root.ReplaceNode(ifStatement, blockStatement).WithAdditionalAnnotations(Formatter.Annotation)
                    : root.ReplaceNode(ifStatement, blockStatement.Statements).WithAdditionalAnnotations(Formatter.Annotation);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }

        private Task<Solution> RemoveConstantFalseConditionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var ifStatement = statement.Ancestors().OfType<IfStatementSyntax>().First();
            var blockStatement = ifStatement.Else?.Statement as BlockSyntax;

            SyntaxNode newRoot;

            // all `if` conditions without being directly in a parent `else`
            if (!ifStatement.Parent.IsKind(SyntaxKind.ElseClause))
            {
                // no else
                newRoot =
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
            }
            else
            {
                newRoot = root.RemoveNode(ifStatement.Parent, SyntaxRemoveOptions.KeepLeadingTrivia & SyntaxRemoveOptions.KeepTrailingTrivia);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}