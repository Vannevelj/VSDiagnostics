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

        private async Task<Document> UseAsAsync(Document document, SyntaxNode statement)
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
                                           .Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword))
                                           .ToArray();

            var castExpressions = ifStatement.Statement
                                             .DescendantNodes()
                                             .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                             .Where(x => !(x is IfStatementSyntax))
                                             .OfType<CastExpressionSyntax>()
                                             .ToArray();

            // Editor is created outside the loop so we can apply multiple fixes to one "document"
            // In our scenario this boils down to applying one fix for every if statement
            var editor = await DocumentEditor.CreateAsync(document);
            var conditionAlreadyReplaced = false;
            var variableAlreadyExtracted = false;

            foreach (var asExpression in asExpressions)
            {
                var identifier = asExpression.Left as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.ValueText != isIdentifier)
                {
                    continue;
                }

                var newIdentifier = SyntaxFactory.Identifier(GetNewIdentifier(isIdentifier, (TypeSyntax) asExpression.Right, semanticModel));

                // Replace condition if it hasn't happened yet
                ReplaceCondition(newIdentifier.ValueText, isExpression, editor, ref conditionAlreadyReplaced);

                // Create as statement before if block
                InsertNewVariableDeclaration(
                    asExpression: asExpression,
                    newIdentifier: newIdentifier,
                    nodeLocation: ifStatement,
                    editor: editor,
                    variableAlreadyExtracted: ref variableAlreadyExtracted);

                // If the expression does not have a variable declarator as parent, we just swap the entire expression for the newly generated identifier
                ReplaceIdentifier(asExpression, newIdentifier, editor);

                // Remove the local variable
                // If the expression is surrounded by an invocation we just swap the expression for the identifier
                // e.g.: bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(o as string);
                if (!asExpression.Ancestors().OfType<InvocationExpressionSyntax>().Any())
                {
                    RemoveLocal(asExpression, editor);
                }
            }

            foreach (var castExpression in castExpressions)
            {
                var identifier = castExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.ValueText != isIdentifier)
                {
                    continue;
                }

                var castedType = semanticModel.GetTypeInfo(castExpression.Type);
                var newIdentifier = SyntaxFactory.Identifier(GetNewIdentifier(isIdentifier, castExpression.Type, semanticModel));

                // Replace condition if it hasn't happened yet
                ReplaceCondition(newIdentifier.ValueText, isExpression, editor, ref conditionAlreadyReplaced);

                // Create as statement before if block
                var typeToCast = castedType.Type.IsNullable() || castedType.Type.IsReferenceType ? castExpression.Type : SyntaxFactory.NullableType(castExpression.Type);
                var newAsClause = SyntaxFactory.BinaryExpression(SyntaxKind.AsExpression, castExpression.Expression, typeToCast);
                InsertNewVariableDeclaration(
                    asExpression: newAsClause,
                    newIdentifier: newIdentifier,
                    nodeLocation: ifStatement,
                    editor: editor,
                    variableAlreadyExtracted: ref variableAlreadyExtracted);

                // If the expression does not have a variable declarator as parent, we just swap the entire expression for the newly generated identifier
                ReplaceIdentifier(castExpression, newIdentifier, editor);

                // Remove the local variable
                // If the expression is surrounded by an invocation we just swap the expression for the identifier
                // e.g.: bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(o as string);
                if (!castExpression.Ancestors().OfType<InvocationExpressionSyntax>().Any())
                {
                    RemoveLocal(castExpression, editor);
                }
            }

            return editor.GetChangedDocument();
        }

        private void ReplaceCondition(string newIdentifier, SyntaxNode isExpression, DocumentEditor editor, ref bool conditionAlreadyReplaced)
        {
            if (conditionAlreadyReplaced)
            {
                return;
            }

            var newCondition = SyntaxFactory.ParseExpression($"{newIdentifier} != null").WithAdditionalAnnotations(Formatter.Annotation);
            editor.ReplaceNode(isExpression, newCondition);
            conditionAlreadyReplaced = true;
            var newDoc = editor.GetChangedDocument();
        }

        private string GetNewIdentifier(string currentIdentifier, TypeSyntax type, SemanticModel semanticModel)
        {
            var nullableType = type as NullableTypeSyntax;
            var typeName = nullableType != null
                ? semanticModel.GetTypeInfo(nullableType.ElementType).Type.Name
                : semanticModel.GetTypeInfo(type).Type.Name;

            return $"{currentIdentifier}As{typeName}";
        }

        private void RemoveLocal(ExpressionSyntax expression, DocumentEditor editor)
        {
            var variableDeclaration = expression.Ancestors().OfType<VariableDeclarationSyntax>().FirstOrDefault();
            if (variableDeclaration == null)
            {
                return;
            }

            if (variableDeclaration.Variables.Count > 1)
            {
                // Remove the appropriate variabledeclarator
                var declaratorToRemove = expression.Ancestors().OfType<VariableDeclaratorSyntax>().First();
                editor.RemoveNode(declaratorToRemove);
            }
            else
            {
                // Remove the entire variabledeclaration
                editor.RemoveNode(variableDeclaration.Ancestors().OfType<LocalDeclarationStatementSyntax>().First());
            }
            var newDoc = editor.GetChangedDocument();
        }

        private void ReplaceIdentifier(ExpressionSyntax expression, SyntaxToken newIdentifier, DocumentEditor editor)
        {
            if (expression.Ancestors().OfType<InvocationExpressionSyntax>().Any())
            {
                editor.ReplaceNode(expression, SyntaxFactory.IdentifierName(newIdentifier));
                return;
            }

            if (!expression.Ancestors().OfType<VariableDeclaratorSyntax>().Any())
            {
                editor.ReplaceNode(expression, SyntaxFactory.IdentifierName(newIdentifier));
            }
            var newDoc = editor.GetChangedDocument();
        }

        private void InsertNewVariableDeclaration(
            BinaryExpressionSyntax asExpression,
            SyntaxToken newIdentifier,
            SyntaxNode nodeLocation,
            DocumentEditor editor,
            ref bool variableAlreadyExtracted)
        {
            if (variableAlreadyExtracted)
            {
                return;
            }

            var newEqualsClause = SyntaxFactory.EqualsValueClause(asExpression);
            var newDeclarator = SyntaxFactory.VariableDeclarator(newIdentifier.WithAdditionalAnnotations(RenameAnnotation.Create()), null, newEqualsClause);
            var newDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"), SyntaxFactory.SeparatedList(new[] { newDeclarator }));
            var newLocal = SyntaxFactory.LocalDeclarationStatement(newDeclaration).WithAdditionalAnnotations(Formatter.Annotation);
            editor.InsertBefore(nodeLocation, newLocal);
            variableAlreadyExtracted = true;
            var newDoc = editor.GetChangedDocument();
        }
    }
}