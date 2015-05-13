using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifier
{
    [ExportCodeFixProvider("TestMethodWithoutPublicModifier", LanguageNames.CSharp), Shared]
    public class TestMethodWithoutPublicModifierCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(TestMethodWithoutPublicModifierAnalyzer.DiagnosticId);
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterFix(CodeAction.Create("Make public", x => MakePublicAsync(context.Document, root, methodDeclaration)), diagnostic);
        }

        private Task<Solution> MakePublicAsync(Document document, SyntaxNode root, MethodDeclarationSyntax method)
        {
            var removableModifiers = new[]
            {
                SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
            };

            var modifierList = new SyntaxTokenList()
                .Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddRange(method.Modifiers.Where(x => !removableModifiers.Select(y => y.RawKind).Contains(x.RawKind)));

            var newMethod = method.WithModifiers(modifierList).WithAdditionalAnnotations(Formatter.Annotation);

            var newRoot = root.ReplaceNode(method, newMethod);
            var formattedRoot = Formatter.Format(newRoot, Formatter.Annotation, document.Project.Solution.Workspace);
            var newDocument = document.WithSyntaxRoot(formattedRoot);

            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}