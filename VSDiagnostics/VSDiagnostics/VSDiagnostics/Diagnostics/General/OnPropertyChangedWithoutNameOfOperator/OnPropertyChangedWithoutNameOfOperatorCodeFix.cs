using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator
{
    [ExportCodeFixProvider(DiagnosticId.OnPropertyChangedWithoutNameofOperator + "CF", LanguageNames.CSharp), Shared]
    public class OnPropertyChangedWithoutNameOfOperatorCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.OnPropertyChangedWithoutNameOfOperatorCodeFixTitle,
                    x => UseNameOfAsync(context.Document, root, diagnostic),
                    OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.Id), diagnostic);
        }

        private Task<Solution> UseNameOfAsync(Document document, SyntaxNode root, Diagnostic diagnostic)
        {
            var propertyName = diagnostic.Properties["parameterName"];
            var startLocation = int.Parse(diagnostic.Properties["startLocation"]);

            // We have to use LastOrDefault because encompassing nodes will have the same start location
            // For example in our scenario of OnPropertyChanged("test"), the ArgumentSyntaxNode will have 
            // the same start location as the following LiteralExpressionNode
            // We are interested in the inner-most node therefore we need to take the last one with that start location
            var nodeToReplace = root.DescendantNodesAndSelf().LastOrDefault(x => x.SpanStart == startLocation);

            var newRoot = root.ReplaceNode(nodeToReplace, SyntaxFactory.ParseExpression($"nameof({propertyName})"));
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}