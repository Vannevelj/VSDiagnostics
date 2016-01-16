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
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull
{
    [ExportCodeFixProvider(nameof(TryCastWithoutUsingAsNotNullCodeFix), LanguageNames.CSharp), Shared]
    public class TryCastWithoutUsingAsNotNullCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(TryCastWithoutUsingAsNotNullAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.TryCastWithoutUsingAsNotNullCodeFixTitle,
                    x => UseAsAsync(context.Document, statement), TryCastWithoutUsingAsNotNullAnalyzer.Rule.Id),
                diagnostic);
        }

        private async Task<Solution> UseAsAsync(Document document, SyntaxNode statement)
        {
            var isExpression = (BinaryExpressionSyntax) statement;
            var isIdentifier = ((IdentifierNameSyntax) isExpression.Left).Identifier.ValueText;
            var ifStatement = statement.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            SemanticModel semanticModel;
            document.TryGetSemanticModel(out semanticModel);

            // We filter out the descendent if statements to avoid executing the code fix on all nested ifs
            var asExpressions = ifStatement.Statement
                                           .DescendantNodes()
                                           .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                           .Where(x => !(x is IfStatementSyntax))
                                           .OfType<BinaryExpressionSyntax>()
                                           .Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword));

            var castExpressions = ifStatement.Statement
                                             .DescendantNodes()
                                             .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                             .Where(x => !(x is IfStatementSyntax))
                                             .OfType<CastExpressionSyntax>();

            // Editor is created outside the loop so we can apply multiple fixes to one "document"
            // In our scenario this boils down to applying one fix for every if statement
            var editor = await DocumentEditor.CreateAsync(document);
            var isConditionReplaced = false;

            // First we check whether there is an applicable scenario in our if condition itself
            // e.g.: if (o is string && ((string) o).Length > 1)
            foreach (
                var expression in
                    ifStatement.Condition.DescendantNodesAndSelf()
                               .Where(x => x is CastExpressionSyntax || (x is BinaryExpressionSyntax && ((BinaryExpressionSyntax) x).OperatorToken.IsKind(SyntaxKind.AsKeyword))))
            {
                var asExpression = expression as BinaryExpressionSyntax;
                var castExpression = expression as CastExpressionSyntax;
                var castedType = semanticModel.GetTypeInfo(asExpression != null ? asExpression.Right : castExpression.Type);

                // We create a new variable name with format identifierAsType
                // In the above example that would become oAsString
                if (!isConditionReplaced)
                {
                    
                    string newIdentifier = $"{isIdentifier}As{castedType.Type.Name}"; ;
                    var newCondition = SyntaxFactory.ParseExpression($"{newIdentifier} != null").WithAdditionalAnnotations(Formatter.Annotation);
                    editor.ReplaceNode(isExpression, newCondition);
                    isConditionReplaced = true;
                }

                // Create a variable outside the if statement and use the as cast
                // We also apply the renamer annotation so the user immediately gets to give it a different name at the point of declaration
                VariableDeclarationSyntax newDeclaration;
                if (asExpression != null)
                {
                    //newDeclaration = SyntaxFactory.VariableDeclaration(asExpression.Right.DescendantNodesAndSelf().OfType<Type>(),)
                    //TODO: find out the type of that inline as
                    // Add test case with a cast to a self defined type
                    // Don't separate this -- combine it with the rest so we can support the CastInSeparateExpression test case 
                }
            }

            // All the body statements
            foreach (var expression in castExpressions.Concat<ExpressionSyntax>(asExpressions))
            {
                // Verifying that we're actually assigning to a variable
                var variableDeclarator = expression.Ancestors().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
                if (variableDeclarator == null)
                {
                    continue;
                }

                IdentifierNameSyntax localVariableBeingCast = null;
                var castedIdentifier = variableDeclarator.Identifier.ValueText;

                var asExpression = expression as BinaryExpressionSyntax;
                if (asExpression != null)
                {
                    localVariableBeingCast = (IdentifierNameSyntax) asExpression.Left;
                }

                var castExpression = expression as CastExpressionSyntax;
                if (castExpression != null)
                {
                    localVariableBeingCast = (IdentifierNameSyntax) castExpression.Expression;
                }

                if (localVariableBeingCast == null || localVariableBeingCast.Identifier.ValueText != isIdentifier)
                {
                    continue;
                }

                // Change if condition
                // This should only happen once
                if (!isConditionReplaced)
                {
                    var newCondition = SyntaxFactory.ParseExpression($"{castedIdentifier} != null").WithAdditionalAnnotations(Formatter.Annotation);
                    editor.ReplaceNode(isExpression, newCondition);
                    isConditionReplaced = true;
                }

                // Create as statement before if block
                var existingDeclaration = variableDeclarator.Ancestors().OfType<VariableDeclarationSyntax>().FirstOrDefault();
                VariableDeclarationSyntax newDeclaration;
                if (asExpression != null)
                {
                    // The existing local was an as statement
                    var newDeclarator = variableDeclarator.WithIdentifier(variableDeclarator.Identifier.WithAdditionalAnnotations(RenameAnnotation.Create()));
                    newDeclaration = SyntaxFactory.VariableDeclaration(existingDeclaration.Type, SyntaxFactory.SeparatedList(new[] { newDeclarator }));
                }
                else
                {
                    // The existing local was a direct cast
                    var castedType = semanticModel.GetTypeInfo(castExpression.Type);
                    var typeToCast = castedType.Type.IsNullable() || castedType.Type.IsReferenceType ? castExpression.Type : SyntaxFactory.NullableType(castExpression.Type);

                    var newAsClause = SyntaxFactory.BinaryExpression(SyntaxKind.AsExpression, castExpression.Expression, typeToCast);
                    var newEqualsClause = SyntaxFactory.EqualsValueClause(newAsClause);
                    var newDeclarator = SyntaxFactory.VariableDeclarator(variableDeclarator.Identifier.WithAdditionalAnnotations(RenameAnnotation.Create()), null, newEqualsClause);
                    newDeclaration = SyntaxFactory.VariableDeclaration(existingDeclaration.Type, SyntaxFactory.SeparatedList(new[] { newDeclarator }));
                }

                var newLocal = SyntaxFactory.LocalDeclarationStatement(newDeclaration).WithAdditionalAnnotations(Formatter.Annotation);
                editor.InsertBefore(ifStatement, newLocal);

                // Rewrite body to remove variable declarator (and declaration if necessary)
                if (existingDeclaration.Variables.Count > 1)
                {
                    // Multiple variables defined in one line e.g.
                    // int x, y = 1;
                    editor.RemoveNode(variableDeclarator);
                }
                else
                {
                    editor.RemoveNode(existingDeclaration.Ancestors().OfType<LocalDeclarationStatementSyntax>().First());
                }
            }

            var newDocument = editor.GetChangedDocument();
            return newDocument.Project.Solution;
        }
    }
}