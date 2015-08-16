using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute;

namespace VSDiagnostics.Diagnostics.Attributes.OnPropertyChangedWithoutCallerMemberName
{
    [ExportCodeFixProvider("OnPropertyChangedWithoutCallerMemberName", LanguageNames.CSharp), Shared]
    public class OnPropertyChangedWithoutCallerMemberNameCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(EnumCanHaveFlagsAttributeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.OnPropertyChangedWithoutCallerMemberNameCodeFixTitle, x => AddCallerMemberNameAttribute(context.Document, root, statement), nameof(OnPropertyChangedWithoutCallerMemberNameAnalyzer)), diagnostic);
        }

        private async Task<Solution> AddCallerMemberNameAttribute(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var editor = await DocumentEditor.CreateAsync(document);

            var methodDeclaration = (MethodDeclarationSyntax)statement;
            var param = methodDeclaration.ParameterList.Parameters.First();

            var callerMemberNameAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("CallerMemberName"));
            var attributeList = SyntaxFactory.AttributeList().AddAttributes(callerMemberNameAttribute);

            var newParam = param.Default == null
                ? param.WithAttributeLists(param.AttributeLists.Add(attributeList))
                    .WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression("\"\"")))
                : param.WithAttributeLists(param.AttributeLists.Add(attributeList));

            editor.ReplaceNode(param, newParam);

            var methodInvocations =
                GetContainingClass(methodDeclaration).DescendantNodes()
                    .OfType<InvocationExpressionSyntax>().Where(i =>
                    {
                        var identifierExpression = i.Expression as IdentifierNameSyntax;
                        return identifierExpression != null && identifierExpression.Identifier.ValueText == "OnPropertyChanged";
                    });

            foreach (var methodInvocation in methodInvocations)
            {
                editor.ReplaceNode(methodInvocation,
                    methodInvocation.WithArgumentList(SyntaxFactory.ArgumentList()));
            }

            var newRoot = await editor.GetChangedDocument().GetSyntaxRootAsync().ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)newRoot;

            var usingSystemRuntimeCompilerServicesDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.CompilerServices"));
            var usingDirectives = compilationUnit.Usings.Select(u => u.Name.GetText().ToString());

            if (usingDirectives.All(u => u != usingSystemRuntimeCompilerServicesDirective.Name.GetText().ToString()))
            {
                var usings = compilationUnit.Usings.Add(usingSystemRuntimeCompilerServicesDirective).OrderBy(u => u.Name.GetText().ToString());

                newRoot = newRoot.ReplaceNode(compilationUnit, compilationUnit.WithUsings(SyntaxFactory.List(usings))
                    .WithAdditionalAnnotations(Formatter.Annotation));
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument.Project.Solution;
        }

        private ClassDeclarationSyntax GetContainingClass(MethodDeclarationSyntax method)
        {
            var parentNode = method.Parent;
            while(true)
            {
                if (parentNode is ClassDeclarationSyntax)
                {
                    return (ClassDeclarationSyntax) parentNode;
                }

                parentNode = parentNode.Parent;
            }
        }
    }
}