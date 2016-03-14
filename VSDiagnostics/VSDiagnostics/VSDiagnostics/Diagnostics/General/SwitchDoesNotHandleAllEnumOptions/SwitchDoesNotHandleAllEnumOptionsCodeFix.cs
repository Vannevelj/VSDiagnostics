using System;
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
using VSDiagnostics.Diagnostics.General.SingleEmptyConstructor;

namespace VSDiagnostics.Diagnostics.General.SwitchDoesNotHandleAllEnumOptions
{
    [ExportCodeFixProvider(nameof(SingleEmptyConstructorCodeFix), LanguageNames.CSharp), Shared]
    internal class SwitchDoesNotHandleAllEnumOptionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(SingleEmptyConstructorAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.SwitchDoesNotHandleAllEnumOptionsCodeFixTitle,
                    x => AddMissingCaseAsync(context.Document, root, statement),
                    SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.Id), diagnostic);
        }

        private async Task<Solution> AddMissingCaseAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var semanticModel = await document.GetSemanticModelAsync();

            var switchBlock = (SwitchStatementSyntax)statement;

            var enumType = semanticModel.GetTypeInfo(switchBlock.Expression).Type as INamedTypeSymbol;
            var caseLabels = switchBlock.Sections.SelectMany(l => l.Labels)
                    .OfType<CaseSwitchLabelSyntax>()
                    .Select(l => l.Value)
                    .ToList();

            // these are the labels like `MyEnum.EnumMember`
            var labels = caseLabels
                    .OfType<MemberAccessExpressionSyntax>()
                    .Select(l => l.Name.Identifier.ValueText)
                    .ToList();

            // these are the labels like `EnumMember` (such as when using `using static Namespace.MyEnum;`)
            labels.AddRange(caseLabels.OfType<IdentifierNameSyntax>().Select(l => l.Identifier.ValueText).ToList());

            var missingLabels = enumType.MemberNames.Where(m => !labels.Contains(m) && !m.StartsWith(".")); // don't create members like ".ctor"

            var newSections = SyntaxFactory.List(switchBlock.Sections);

            foreach (var label in missingLabels)
            {
                var caseLabel =
                    SyntaxFactory.CaseSwitchLabel(
                        SyntaxFactory.ParseExpression(" " + enumType.Name + "." + label)
                            .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia(Environment.NewLine)));

                var notImplementedException =
                    SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression(" new System.NotImplementedException()" + Environment.NewLine));
                var statements = SyntaxFactory.List(new List<StatementSyntax> {notImplementedException});

                var section = SyntaxFactory.SwitchSection(SyntaxFactory.List(new List<SwitchLabelSyntax> {caseLabel}), statements);

                newSections = newSections.Insert(0, section);
            }

            var newRoot = root.ReplaceNode(switchBlock, switchBlock.WithSections(newSections).WithAdditionalAnnotations(Formatter.Annotation));
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument.Project.Solution;
        }
    }
}