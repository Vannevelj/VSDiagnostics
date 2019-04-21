using System;
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
using Microsoft.CodeAnalysis.Simplification;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.DateTimeNow
{
    [ExportCodeFixProvider(DiagnosticId.DateTimeNow + "CF", LanguageNames.CSharp), Shared]
    public class DateTimeNowCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DateTimeNowAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan).DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.NewGuidUseNewGuidCodeFixTitle,
                    x => UseUtc(context.Document, root, statement), DateTimeNowAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Document> UseUtc(Document document, SyntaxNode root, MemberAccessExpressionSyntax statement)
        {
            var newRoot = root.ReplaceNode(statement, SyntaxFactory.ParseExpression("System.DateTime.UtcNow").WithAdditionalAnnotations(Simplifier.Annotation));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
