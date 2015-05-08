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

namespace VSDiagnostics.Diagnostics.General.NullableToShorthand
{
    [ExportCodeFixProvider("NullableToShorthand", LanguageNames.CSharp), Shared]
    public class NullableToShorthandCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(NullableToShorthandAnalyzer.DiagnosticId);
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

            var declaration = root.FindToken(diagnosticSpan.Start);
            context.RegisterFix(CodeAction.Create("Use shorthand notation", x => UseShorthandNotation(context.Document, root, declaration)), diagnostic);
        }

        private static async Task<Solution> UseShorthandNotation(Document document, SyntaxNode root, SyntaxToken declaration)
        {
            var node = root.FindNode(declaration.Span);
            var typeNode = (GenericNameSyntax) node;
            var semanticModel = await document.GetSemanticModelAsync();
            var type = semanticModel.GetTypeInfo(typeNode.TypeArgumentList.Arguments.First()).Type;
            var typeNameString = type.MetadataName;

            if (!type.IsExtern)
            {
                switch (type.MetadataName)
                {
                    case "Int32":
                        typeNameString = "int";
                        break;
                    case "UInt32":
                        typeNameString = "uint";
                        break;
                    case "Int64":
                        typeNameString = "long";
                        break;
                    case "UInt64":
                        typeNameString = "ulong";
                        break;
                    case "Int16":
                        typeNameString = "short";
                        break;
                    case "UInt16":
                        typeNameString = "ushort";
                        break;
                    case "Single":
                        typeNameString = "float";
                        break;
                    case "Double":
                        typeNameString = "double";
                        break;
                    case "Decimal":
                        typeNameString = "decimal";
                        break;
                }
            }

            var newExpression = SyntaxFactory.ParseTypeName(typeNameString + "?")
                                             .WithLeadingTrivia(node.GetLeadingTrivia())
                                             .WithTrailingTrivia(node.GetTrailingTrivia());

            var newParent = node.ReplaceNode(node, newExpression);
            var newRoot = root.ReplaceNode(node, newParent);
            var formattedRoot = Formatter.Format(newRoot, newParent.Span, document.Project.Solution.Workspace, document.Project.Solution.Workspace.Options);
            var newDocument = document.WithSyntaxRoot(formattedRoot);

            return newDocument.Project.Solution;
        }
    }
}