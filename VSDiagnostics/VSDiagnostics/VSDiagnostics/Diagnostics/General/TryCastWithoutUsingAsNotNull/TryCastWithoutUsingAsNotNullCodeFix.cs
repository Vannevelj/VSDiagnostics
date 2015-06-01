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
            context.RegisterCodeFix(CodeAction.Create("Use as", x => UseAs(context.Document, root, statement)), diagnostic);
        }

        private async Task<Solution> UseAs(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var isExpression = (BinaryExpressionSyntax) statement;
            var ifStatement = statement.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            var asExpressions = ifStatement.Statement.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>().Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword));
            var isIdentifier = ((IdentifierNameSyntax) isExpression.Left).Identifier.ValueText;

            foreach (var asExpression in asExpressions)
            {
                var asIdentifier = ((IdentifierNameSyntax) asExpression.Left).Identifier.ValueText;
                if (string.Equals(isIdentifier, asIdentifier))
                {
                    // Move the as statement before the if block
                    // Change the if-condition to "NewAsIdentifier != null"

                    var variableDeclarator = asExpression.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
                    var newAsIdentifier = variableDeclarator.Identifier.ValueText;
                    var newCondition = SyntaxFactory.ParseExpression($"{newAsIdentifier} != null");

                    var variableDeclaration = asExpression.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

                    var editor = await DocumentEditor.CreateAsync(document);
                    editor.RemoveNode(variableDeclaration);
                    editor.ReplaceNode(ifStatement.Condition, newCondition);
                    editor.InsertBefore(ifStatement, new[] { variableDeclaration.WithAdditionalAnnotations(Formatter.Annotation) });

                    var newDocument = editor.GetChangedDocument();
                    return newDocument.Project.Solution;
                }
            }

            return null;
        }
    }
}