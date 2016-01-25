using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
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
                    x => UseAsAsync(context.Document, statement, context.CancellationToken), TryCastWithoutUsingAsNotNullAnalyzer.Rule.Id),
                diagnostic);
        }

        private async Task<Document> UseAsAsync(Document document, SyntaxNode statement, CancellationToken cancellationToken)
        {
            // Editor is created outside the loop so we can apply multiple fixes to one "document"
            // In our scenario this boils down to applying one fix for every if statement
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var isExpression = (BinaryExpressionSyntax) statement;
            var isIdentifier = ((IdentifierNameSyntax) isExpression.Left).Identifier.ValueText;
            var newIdentifier = SyntaxFactory.Identifier(GetNewIdentifier(isIdentifier, (TypeSyntax) isExpression.Right, semanticModel));
            var ifStatement = statement.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            // We filter out the descendent if statements to avoid executing the code fix on all nested ifs
            var asExpressions = GetDescendantBinaryAs(ifStatement);
            var castExpressions = GetDescendantCasts(ifStatement);

            editor.ReplaceNode(isExpression, isExpression.WithAdditionalAnnotations(new SyntaxAnnotation("MyIsStatement")));

            // First step: we give every eligible expression a custom annotation to indicate the identifiers that need to be renamed
            var documentId = document.Id;
            var projectId = document.Project.Id;
            foreach (var expression in asExpressions.Concat<ExpressionSyntax>(castExpressions))
            {
                var identifier = default(IdentifierNameSyntax);
                var binaryExpression = expression as BinaryExpressionSyntax;
                if (binaryExpression != null)
                {
                    identifier = binaryExpression.Left as IdentifierNameSyntax;
                }

                var castExpression = expression as CastExpressionSyntax;
                if (castExpression != null)
                {
                    identifier = castExpression.Expression as IdentifierNameSyntax;
                }

                if (identifier != null && identifier.Identifier.ValueText == isIdentifier)
                {
                    // !!Important!!
                    // We add the annotation on top of the identifier we find but this is *not* the identifier we want to rename
                    // var myVar x = (int) o;
                    // Here we place the annotation on "o" but we want to rename "x"
                    // We can't place it on "x" because VariableDeclarators can't be replaced using DocumentEditor just yet
                    // See https://github.com/dotnet/roslyn/issues/8154 for more info
                    if (identifier.Ancestors().OfType<VariableDeclaratorSyntax>().Any())
                    {
                        editor.ReplaceNode(identifier, identifier.WithAdditionalAnnotations(new SyntaxAnnotation("QueueRename")));
                    }
                }
            }
            document = editor.GetChangedDocument();

            // Second step: rename all identifiers
            while (true)
            {
                var root = await document.GetSyntaxRootAsync(cancellationToken);
                var tempSemanticModel = await document.GetSemanticModelAsync(cancellationToken);
                VariableDeclaratorSyntax nodeToRename = null;
                foreach (var node in root.GetAnnotatedNodes("QueueRename").Cast<IdentifierNameSyntax>())
                {
                    var declarator = node.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().Single();
                    // We have to find the first node that isn't renamed yet
                    if (declarator.Identifier.ValueText != newIdentifier.ValueText)
                    {
                        // We only rename the VariableDeclarators that are directly assigned the casted variable in question
                        // If the variable is used in a separate expression, we don't have to rename our declarator
                        if (!IsSurroundedByInvocation(node))
                        {
                            nodeToRename = declarator;
                            break;
                        }
                    }
                }
                if (nodeToRename == null)
                {
                    break;
                }
                var nodeToRenameSymbol = tempSemanticModel.GetDeclaredSymbol(nodeToRename);
                if (nodeToRenameSymbol == null)
                {
                    break;
                }


                var renamedSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, nodeToRenameSymbol, newIdentifier.ValueText, null, cancellationToken);
                document = renamedSolution.Projects.Single(x => x.Id == projectId).GetDocument(documentId);
            }

            // Third step: use the newly created document
            editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var newRoot = await document.GetSyntaxRootAsync(cancellationToken);
            isExpression = (BinaryExpressionSyntax) newRoot.GetAnnotatedNodes("MyIsStatement").Single();
            ifStatement = isExpression.Ancestors().OfType<IfStatementSyntax>().First();
            asExpressions = GetDescendantBinaryAs(ifStatement);
            castExpressions = GetDescendantCasts(ifStatement);

            var conditionAlreadyReplaced = false;
            var variableAlreadyExtracted = false;

            foreach (var asExpression in asExpressions)
            {
                var identifier = asExpression.Left as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.ValueText != isIdentifier)
                {
                    continue;
                }

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
                if (!IsSurroundedByInvocation(asExpression))
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
                if (!IsSurroundedByInvocation(castExpression))
                {
                    RemoveLocal(castExpression, editor);
                }
            }

            return editor.GetChangedDocument();
        }

        private bool IsSurroundedByInvocation(ExpressionSyntax expression)
        {
            return expression.Ancestors().OfType<InvocationExpressionSyntax>().Any();
        }

        private IEnumerable<CastExpressionSyntax> GetDescendantCasts(IfStatementSyntax ifStatement)
        {
            return ifStatement.Statement.DescendantNodes()
                              .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                              .Where(x => !(x is IfStatementSyntax))
                              .OfType<CastExpressionSyntax>()
                              .ToArray();
        }

        private IEnumerable<BinaryExpressionSyntax> GetDescendantBinaryAs(IfStatementSyntax ifStatement)
        {
            return ifStatement.Statement
                              .DescendantNodes()
                              .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                              .Where(x => !(x is IfStatementSyntax))
                              .OfType<BinaryExpressionSyntax>()
                              .Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword))
                              .ToArray();
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
        }
    }
}