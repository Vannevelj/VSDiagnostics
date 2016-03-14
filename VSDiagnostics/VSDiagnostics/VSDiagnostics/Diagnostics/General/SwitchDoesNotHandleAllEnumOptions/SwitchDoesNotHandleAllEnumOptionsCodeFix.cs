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
using Microsoft.CodeAnalysis.Simplification;

namespace VSDiagnostics.Diagnostics.General.SwitchDoesNotHandleAllEnumOptions
{
    [ExportCodeFixProvider(nameof(SwitchDoesNotHandleAllEnumOptionsCodeFix), LanguageNames.CSharp), Shared]
    internal class SwitchDoesNotHandleAllEnumOptionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.Id);

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
            var qualifier = "System.";
            var usingSystemDirective =
                ((CompilationUnitSyntax) root).Usings.Where(u => u.Name is IdentifierNameSyntax)
                    .FirstOrDefault(u => ((IdentifierNameSyntax) u.Name).Identifier.ValueText == "System");

            if (usingSystemDirective != null)
            {
                qualifier = usingSystemDirective.Alias == null
                    ? string.Empty
                    : usingSystemDirective.Alias.Name.Identifier.ValueText + ".";
            }

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

            // use simplified form if there are any in simplified form or if there are not any labels at all
            var useSimplifiedForm = caseLabels.OfType<IdentifierNameSyntax>().Any() ||
                                    !caseLabels.OfType<MemberAccessExpressionSyntax>().Any();

            // don't create members like ".ctor"
            var missingLabels = enumType.MemberNames.Where(m => !labels.Contains(m) && !m.StartsWith("."));

            var newSections = SyntaxFactory.List(switchBlock.Sections);

            var notImplementedException =
                SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression($" new {qualifier}NotImplementedException()"))
                    .WithAdditionalAnnotations(Simplifier.Annotation);

            foreach (var label in missingLabels)
            {
                var caseLabel =
                    SyntaxFactory.CaseSwitchLabel(
                        SyntaxFactory.ParseExpression(" " + enumType.Name + "." + label)
                            .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia(Environment.NewLine)));
                
                var statements = SyntaxFactory.List(new List<StatementSyntax> {notImplementedException.WithAdditionalAnnotations(Simplifier.Annotation)});

                var section =
                    SyntaxFactory.SwitchSection(SyntaxFactory.List(new List<SwitchLabelSyntax> {caseLabel}), statements)
                        .WithAdditionalAnnotations(Formatter.Annotation);

                newSections = newSections.Insert(0, section);
            }

            var newNode = useSimplifiedForm
                ? switchBlock.WithSections(newSections).WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation)
                : switchBlock.WithSections(newSections).WithAdditionalAnnotations(Formatter.Annotation);

            var newRoot = root.ReplaceNode(switchBlock, newNode);
            var newDocument = await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot));
            return newDocument.Project.Solution;
        }
    }
}