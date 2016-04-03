using System;
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
    [ExportCodeFixProvider(DiagnosticId.TryCastWithoutUsingAsNotNull + "CF", LanguageNames.CSharp), Shared]
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

        private async Task<Document> UseAsAsync(Document document, SyntaxNode statement,
            CancellationToken cancellationToken)
        {
            var documentId = document.Id;
            var projectId = document.Project.Id;
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            editor.ReplaceNode(statement, statement.WithAdditionalAnnotations(new SyntaxAnnotation("MyIsStatement")));

            // Remove all existing to-rename annotations
            // This is needed because after renaming one branch in an `if` statement, the extraction is done succesfully but the annotation remains on the extracted declarator
            // Therefore we have to remove all appropriate annotations so we can start fresh from the new situation
            // This should only be done for the extracted variable declarator(s): the is-statement is removed so no annotation for that remains
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            foreach (var annotatedNode in root.GetAnnotatedNodes("QueueRename"))
            {
                editor.ReplaceNode(annotatedNode, annotatedNode.WithoutAnnotations("QueueRename"));
            }
            document = editor.GetChangedDocument();

            // Editor is created outside the loop so we can apply multiple fixes to one "document"
            // In our scenario this boils down to applying one fix for every if statement
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            root = await document.GetSyntaxRootAsync(cancellationToken);
            var isExpression = (BinaryExpressionSyntax) root.GetAnnotatedNodes("MyIsStatement").Single();
            var isIdentifier = ((IdentifierNameSyntax) isExpression.Left).Identifier.ValueText;
            var ifStatement = isExpression.Ancestors().OfType<IfStatementSyntax>().First();
            var nullableType = isExpression.Right as NullableTypeSyntax;
            var type = nullableType != null
                ? semanticModel.GetTypeInfo(nullableType.ElementType).Type
                : semanticModel.GetTypeInfo(isExpression.Right).Type;
            var newIdentifier =
                SyntaxFactory.Identifier(GetNewIdentifier(isIdentifier, type, GetOuterIfStatement(ifStatement).Parent));


            // We filter out the descendent if statements to avoid executing the code fix on all nested ifs
            var asExpressions = GetDescendantBinaryAs(ifStatement);
            var castExpressions = GetDescendantCasts(ifStatement);

            // First step: we give every eligible expression a custom annotation to indicate the identifiers that need to be renamed
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
                        editor.ReplaceNode(identifier,
                            identifier.WithAdditionalAnnotations(new SyntaxAnnotation("QueueRename")));
                    }
                }
            }
            document = editor.GetChangedDocument();

            // Second step: rename all identifiers
            while (true)
            {
                root = await document.GetSyntaxRootAsync(cancellationToken);
                VariableDeclaratorSyntax nodeToRename = null;
                foreach (var node in root.GetAnnotatedNodes("QueueRename").Cast<IdentifierNameSyntax>())
                {
                    var declarator = node.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().Single();
                    // We have to find the first node that isn't renamed yet
                    if (declarator.Identifier.ValueText != newIdentifier.ValueText)
                    {
                        // We only rename the VariableDeclarators that are directly assigned the casted variable in question
                        // If the variable is used in a separate expression, we don't have to rename our declarator
                        if (!IsSurroundedByInvocation(node) && !IsInConditionalExpression(node))
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

                var tempSemanticModel = await document.GetSemanticModelAsync(cancellationToken);
                var nodeToRenameSymbol = tempSemanticModel.GetDeclaredSymbol(nodeToRename);
                if (nodeToRenameSymbol == null)
                {
                    break;
                }

                var renamedSolution =
                    await
                        Renamer.RenameSymbolAsync(document.Project.Solution, nodeToRenameSymbol, newIdentifier.ValueText,
                            null, cancellationToken);
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

                var castedType = semanticModel.GetTypeInfo(asExpression.Right);
                if (castedType.Type != type)
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

                ReplaceIdentifier(asExpression, newIdentifier, editor);

                // Remove the local variable
                // If the expression is surrounded by an invocation we just swap the expression for the identifier
                // e.g.: bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(o as string);
                if (!IsSurroundedByInvocation(asExpression) && !IsInConditionalExpression(asExpression))
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
                if (castedType.Type != type)
                {
                    continue;
                }

                // Replace condition if it hasn't happened yet
                ReplaceCondition(newIdentifier.ValueText, isExpression, editor, ref conditionAlreadyReplaced);

                // Create as statement before if block
                var typeToCast = castedType.Type.IsNullable() || castedType.Type.IsReferenceType
                    ? castExpression.Type
                    : SyntaxFactory.NullableType(castExpression.Type);
                var newAsClause = SyntaxFactory.BinaryExpression(SyntaxKind.AsExpression, castExpression.Expression,
                    typeToCast);
                InsertNewVariableDeclaration(
                    asExpression: newAsClause,
                    newIdentifier: newIdentifier,
                    nodeLocation: ifStatement,
                    editor: editor,
                    variableAlreadyExtracted: ref variableAlreadyExtracted);

                // If we have a direct cast (yes) and the existing type was a non-nullable value type, we have to add the `.Value` property accessor ourselves
                // While it is not necessary to add the property access in the case of a nullable collection, we do it anyway because that's a very difficult thing to calculate otherwise
                // e.g. new double?[] { 5.0, 6.0, 7.0 }.Contains(oAsDouble.Value)
                // The above can be written with or without `.Value` when the collection is double?[] but requires `.Value` in the case of double[]
                ReplaceIdentifier(castExpression, newIdentifier, editor,
                    requiresNullableValueAccess: castedType.Type.IsValueType && !castedType.Type.IsNullable());

                // Remove the local variable
                // If the expression is surrounded by an invocation we just swap the expression for the identifier
                // e.g.: bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(o as string);
                if (!IsSurroundedByInvocation(castExpression) && !IsInConditionalExpression(castExpression))
                {
                    RemoveLocal(castExpression, editor);
                }
            }

            return editor.GetChangedDocument();
        }

        private bool IsSurroundedByInvocation(ExpressionSyntax expression) => expression.Ancestors().OfType<InvocationExpressionSyntax>().Any();

        private IEnumerable<CastExpressionSyntax> GetDescendantCasts(IfStatementSyntax ifStatement) => ifStatement.Statement.DescendantNodes()
                                                                                                                  .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                                                                                                  .Where(x => !(x is IfStatementSyntax))
                                                                                                                  .OfType<CastExpressionSyntax>()
                                                                                                                  .ToArray();

        private IEnumerable<BinaryExpressionSyntax> GetDescendantBinaryAs(IfStatementSyntax ifStatement) => ifStatement.Statement
                                                                                                                       .DescendantNodes()
                                                                                                                       .Concat(ifStatement.Condition.DescendantNodesAndSelf())
                                                                                                                       .Where(x => !(x is IfStatementSyntax))
                                                                                                                       .OfType<BinaryExpressionSyntax>()
                                                                                                                       .Where(x => x.OperatorToken.IsKind(SyntaxKind.AsKeyword))
                                                                                                                       .ToArray();

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

        private string GetNewIdentifier(string currentIdentifier, ITypeSymbol type, SyntaxNode context)
        {
            var newName = $"{currentIdentifier}As{type.Name}";

            // We add a suffix counter in case there are naming collisions. 
            var collidingIdentifier = context.DescendantNodesAndSelf()
                                             .OfType<IdentifierNameSyntax>()
                                             .Select(x => x.Identifier.ValueText)
                                             .Where(x => x.StartsWith(newName))
                                             .OrderByDescending(x => x)
                                             .FirstOrDefault();

            if (collidingIdentifier != null)
            {
                var indexOfUnderscore = collidingIdentifier.LastIndexOf('_');
                int index;
                if (indexOfUnderscore > 0 && int.TryParse(collidingIdentifier.Substring(indexOfUnderscore + 1), out index))
                {
                    return $"{newName}_{++index}";
                }
                return $"{newName}_1";
            }

            return newName;
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

        /// <summary>
        ///     Replaces a certain expression for an identifier if the context is appropriate.
        /// </summary>
        /// <param name="expression">The expression (cast or binary as) to be replaced</param>
        /// <param name="newIdentifier">The new identifier that refers to the extracted variable</param>
        /// <param name="editor">The <see cref="DocumentEditor" /> used to edit the tree</param>
        /// <param name="requiresNullableValueAccess">
        ///     <code>true</code> if a <code>.Value</code> property access needs to be added to the new identifier.
        ///     This is needed in the case of a direct cast inside a larger invocation expression that needs to be replaced with an
        ///     identifier.
        ///     If this <code>.Value</code> wouldn't be added, it would create uncompilable code because you've changed the type
        ///     from int to int?.
        ///     Note that this should only happen in the case of a value type.
        /// </param>
        private void ReplaceIdentifier(ExpressionSyntax expression, SyntaxToken newIdentifier, DocumentEditor editor, bool requiresNullableValueAccess = false)
        {
            var newIdentifierName = requiresNullableValueAccess ?
                SyntaxFactory.ParseExpression($"{newIdentifier.ValueText}.Value") :
                SyntaxFactory.IdentifierName(newIdentifier);

            editor.ReplaceNode(expression, newIdentifierName.WithAdditionalAnnotations(Formatter.Annotation));
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

            // If we are in an else statement, we have to add the new local before the initial if-statement. e.g.:
            //   if(o is int) { }
            //   else if(o is string) { }
            // If we are currently handling the second statement, we have to add the local before the first
            // However because there can be multiple chained if-else statements, we have to go up to the first one and add it there.
            nodeLocation = GetOuterIfStatement(nodeLocation);

            editor.InsertBefore(nodeLocation, newLocal);
            variableAlreadyExtracted = true;
        }

        private SyntaxNode GetOuterIfStatement(SyntaxNode nodeLocation)
        {
            while (true)
            {
                var oneTrue = false;

                if (nodeLocation is IfStatementSyntax && nodeLocation.Parent is ElseClauseSyntax)
                {
                    nodeLocation = nodeLocation.Parent;
                    oneTrue = true;
                }

                if (nodeLocation is ElseClauseSyntax && nodeLocation.Parent is IfStatementSyntax)
                {
                    nodeLocation = nodeLocation.Parent;
                    oneTrue = true;
                }

                if (!oneTrue)
                {
                    return nodeLocation;
                }
            }
        }

        private bool IsInConditionalExpression(SyntaxNode expression) => expression.AncestorsAndSelf().Any(x => x.IsKind(SyntaxKind.ConditionalExpression));
    }
}