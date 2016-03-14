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
                    x => AddMissingCaseAsync(context.Document, (CompilationUnitSyntax)root, statement),
                    SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.Id), diagnostic);
        }

        private async Task<Solution> AddMissingCaseAsync(Document document, CompilationUnitSyntax root, SyntaxNode statement)
        {
            var semanticModel = await document.GetSemanticModelAsync();

            var switchBlock = (SwitchStatementSyntax)statement;

            var enumType = semanticModel.GetTypeInfo(switchBlock.Expression).Type as INamedTypeSymbol;
            var caseLabels = switchBlock.Sections.SelectMany(l => l.Labels)
                    .OfType<CaseSwitchLabelSyntax>()
                    .Select(l => l.Value)
                    .ToList();

            var missingLabels = GetMissingLabels(caseLabels, enumType);

            // use simplified form if there are any in simplified form or if there are not any labels at all
            var useSimplifiedForm = (caseLabels.OfType<IdentifierNameSyntax>().Any() ||
                                    !caseLabels.OfType<MemberAccessExpressionSyntax>().Any()) &&
                                    EnumIsUsingStatic(root, enumType);

            var qualifier = GetQualifierForException(root);

            var notImplementedException =
                SyntaxFactory.ThrowStatement(SyntaxFactory.ParseExpression($" new {qualifier}NotImplementedException()"))
                    .WithAdditionalAnnotations(Simplifier.Annotation);
            var statements = SyntaxFactory.List(new List<StatementSyntax> { notImplementedException });

            var newSections = SyntaxFactory.List(switchBlock.Sections);

            foreach (var label in missingLabels)
            {
                // ReSharper disable once PossibleNullReferenceException
                var caseLabel =
                    SyntaxFactory.CaseSwitchLabel(
                        SyntaxFactory.ParseExpression(useSimplifiedForm ? $" {label}" : $" {enumType.Name}.{label}")
                            .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia(Environment.NewLine)));

                var section =
                    SyntaxFactory.SwitchSection(SyntaxFactory.List(new List<SwitchLabelSyntax> {caseLabel}), statements)
                        .WithAdditionalAnnotations(Formatter.Annotation);

                // ensure that the new cases are above the default case
                newSections = newSections.Insert(0, section);
            }

            var newNode = useSimplifiedForm
                ? switchBlock.WithSections(newSections).WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation)
                : switchBlock.WithSections(newSections).WithAdditionalAnnotations(Formatter.Annotation);

            var newRoot = root.ReplaceNode(switchBlock, newNode);
            var newDocument = await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot));
            return newDocument.Project.Solution;
        }

        private bool EnumIsUsingStatic(CompilationUnitSyntax root, INamedTypeSymbol enumType)
        {
            var fullyQualifiedName = enumType.Name;

            var containingNamespace = enumType.ContainingNamespace;
            while (!string.IsNullOrEmpty(containingNamespace.Name))
            {
                fullyQualifiedName = fullyQualifiedName.Insert(0, containingNamespace.Name + ".");
                containingNamespace = containingNamespace.ContainingNamespace;
            }

            return root.Usings.Any(u =>
                {
                    if (!u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)) { return false; }

                    var name = u.Name as QualifiedNameSyntax;
                    if (name == null) { return false; }

                    return new string(name.GetText().ToString().ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray()) == fullyQualifiedName;
                });
        }

        private IEnumerable<string> GetMissingLabels(List<ExpressionSyntax> caseLabels, INamedTypeSymbol enumType)
        {
            // these are the labels like `MyEnum.EnumMember`
            var labels = caseLabels
                .OfType<MemberAccessExpressionSyntax>()
                .Select(l => l.Name.Identifier.ValueText)
                .ToList();

            // these are the labels like `EnumMember` (such as when using `using static Namespace.MyEnum;`)
            labels.AddRange(caseLabels.OfType<IdentifierNameSyntax>().Select(l => l.Identifier.ValueText));

            // don't create members like ".ctor"
            return enumType.MemberNames.Except(labels).Where(m => !m.StartsWith("."));
        }

        private string GetQualifierForException(CompilationUnitSyntax root)
        {
            var qualifier = "System.";
            var usingSystemDirective =
                root.Usings.Where(u => u.Name is IdentifierNameSyntax)
                    .FirstOrDefault(u => ((IdentifierNameSyntax) u.Name).Identifier.ValueText == "System");

            if (usingSystemDirective != null)
            {
                qualifier = usingSystemDirective.Alias == null
                    ? string.Empty
                    : usingSystemDirective.Alias.Name.Identifier.ValueText + ".";
            }
            return qualifier;
        }
    }
}