using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral
{
    [ExportCodeFixProvider("CompareBooleanToTrueLiteral", LanguageNames.CSharp), Shared]
    public class CompareBooleanToTrueLiteralCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CompareBooleanToTrueLiteralAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Simplify expression", x => SimplifyExpressionAsync(context.Document, root, statement)), diagnostic);
        }

        private Task<Solution> SimplifyExpressionAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            throw new NotImplementedException();
        }
    }
}