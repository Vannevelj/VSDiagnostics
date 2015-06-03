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

namespace VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull
{
    [ExportCodeFixProvider("TryCastWithoutUsingAsNotNull", LanguageNames.CSharp), Shared]
    public class TryCastWithoutUsingAsNotNullCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create("Use as", x => UseAsAsync(context.Document, root, statement)), diagnostic);
        }

        private async Task<Solution> UseAsAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var isExpression = (BinaryExpressionSyntax) statement;
            var ifStatement = statement.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            var asExpressions = ifStatement.Statement.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>().Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword));
            var isIdentifier = ((IdentifierNameSyntax) isExpression.Left).Identifier.ValueText;

            foreach (var asExpression in asExpressions)
            {
                var isIdentifierInAsContext = ((IdentifierNameSyntax) asExpression.Left).Identifier.ValueText;
                if (string.Equals(isIdentifier, isIdentifierInAsContext))
                {
                    // Move the as statement before the if block
                    // Change the if-condition to "NewAsIdentifier != null"

                    var variableDeclarator = asExpression.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
                    var asIdentifier = variableDeclarator.Identifier.ValueText;
                    var variableDeclaration = asExpression.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

                    var editor = await DocumentEditor.CreateAsync(document);

                    var newCondition = SyntaxFactory.ParseExpression($"{asIdentifier} != null");
                    editor.ReplaceNode(ifStatement.Condition, newCondition);

                    if (variableDeclaration.Declaration.Variables.Count > 1) // Split variable declaration
                    {
                        // Extract the relevant variable and copy it outside the if-body
                        var extractedDeclarator = variableDeclaration.Declaration.Variables.First(x => x.Identifier.ValueText == asIdentifier);
                        var newDeclaration = SyntaxFactory.VariableDeclaration(
                            extractedDeclarator.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First().Type,
                            SyntaxFactory.SeparatedList(new[] { extractedDeclarator }));
                        var newStatement = SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.TokenList(),
                            newDeclaration,
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                        editor.InsertBefore(ifStatement, new[] { newStatement.WithAdditionalAnnotations(Formatter.Annotation) });

                        // Rewrite the variable declaration inside the if-body to remove the one we just copied
                        var newVariables = variableDeclaration.Declaration.WithVariables(SyntaxFactory.SeparatedList(variableDeclaration.Declaration.Variables.Except(new[] { extractedDeclarator })));
                        var newBodyStatement = SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.TokenList(),
                            newVariables,
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                        editor.ReplaceNode(variableDeclaration, newBodyStatement);
                    }
                    else // Move declaration outside if-body
                    {
                        editor.RemoveNode(variableDeclaration);
                        editor.InsertBefore(ifStatement, new[] { variableDeclaration.WithAdditionalAnnotations(Formatter.Annotation) });
                    }

                    var newDocument = editor.GetChangedDocument();
                    return newDocument.Project.Solution;
                }
            }

            return null;
        }
    }
}