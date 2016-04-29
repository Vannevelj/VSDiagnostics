using System.Collections.Generic;
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

namespace VSDiagnostics.Diagnostics.General.SwitchIsMissingDefaultLabel
{
    [ExportCodeFixProvider(DiagnosticId.SwitchIsMissingDefaultLabel + "CF", LanguageNames.CSharp), Shared]
    internal class SwitchIsMissingDefaultLabelCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(SwitchIsMissingDefaultLabelAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan).Parent;
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.SwitchIsMissingDefaultSectionCodeFixTitle,
                    x => AddDefaultCaseAsync(context.Document, (CompilationUnitSyntax)root, (SwitchStatementSyntax)statement),
                    SwitchIsMissingDefaultLabelAnalyzer.Rule.Id), diagnostic);
        }

        private async Task<Document> AddDefaultCaseAsync(Document document, CompilationUnitSyntax root, SwitchStatementSyntax switchBlock)
        {
            var notImplementedException =
                SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression("new System.ArgumentException()"))
                             .WithAdditionalAnnotations(Simplifier.Annotation, Formatter.Annotation);
            var statements = SyntaxFactory.List(new List<StatementSyntax> { notImplementedException });

            var defaultCase = SyntaxFactory.SwitchSection(SyntaxFactory.List<SwitchLabelSyntax>(new[] { SyntaxFactory.DefaultSwitchLabel() }), statements);

            var newNode = switchBlock.AddSections(defaultCase.WithAdditionalAnnotations(Formatter.Annotation,
                    Simplifier.Annotation));

            var newRoot = root.ReplaceNode(switchBlock, newNode);
            return await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot));
        }
    }
}