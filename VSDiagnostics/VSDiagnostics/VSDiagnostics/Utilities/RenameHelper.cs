// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Found at https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/Helpers/RenameHelper.cs

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Rename;

namespace VSDiagnostics.Utilities
{
    internal static class RenameHelper
    {
        private static readonly HashSet<string> NamingConflictDiagnosticIds = new HashSet<string>(new[]
        {
            "CS0015", // The name of type 'type' is too long
            "CS0041", // The fully qualified name for 'type' is too long for debug information.
            "CS0076", // The enumerator name 'value__' is reserved and cannot be used
            "CS0082", // Type 'type' already reserves a member called 'name' with the same parameter types
            "CS0100", // The parameter name 'parameter name' is a duplicate
            "CS0101", // The namespace 'namespace' already contains a definition for 'type'
            "CS0102", // The type 'type name' already contains a definition for 'identifier'
            "CS0104", // 'reference' is an ambiguous reference between 'identifier' and 'identifier'
            "CS0111", // Type 'class' already defines a member called 'member' with the same parameter types,
            "CS0121", // The call is ambiguous between the following methods or properties: 'method1' and 'method2'
            "CS0128", // A local variable named 'variable' is already defined in this scope
            "CS0135", // 'declaration1' conflicts with the declaration 'declaration2'
            "CS0136", // A local variable named 'var' cannot be declared in this scope (..)
            "CS0140", // The label 'label' is a duplicate
            "CS0158", // The label 'label' shadows another label by the same name in a contained scope
            "CS0202", // foreach requires that the return type 'type' of 'type.GetEnumerator()' must have a (..)
            "CS0229", // Ambiguity between 'member1' and 'member2'
            "CS0316", // The parameter name 'name' conflicts with an automatically-generated parameter name.
            "CS0412", // 'generic': a parameter or local variable cannot have the same name as a method type parameter
            "CS0473", // Explicit interface implementation 'method name' matches more than one interface member.
            "CS0542", // 'user-defined type' : member names cannot be the same as their enclosing type,
            "CS1061", // 'type' does not contain a definition for 'member' and no extension method 'name' accepting (..)
        });

        public static async Task<Solution> RenameSymbolAsync(Document document, SyntaxNode root, SyntaxToken declarationToken, string newName, CancellationToken cancellationToken)
        {
            var annotatedRoot = root.ReplaceToken(declarationToken, declarationToken.WithAdditionalAnnotations(RenameAnnotation.Create()));
            var annotatedSolution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, annotatedRoot);
            var annotatedDocument = annotatedSolution.GetDocument(document.Id);

            annotatedRoot = await annotatedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var annotatedToken = annotatedRoot.FindToken(declarationToken.SpanStart);

            var semanticModel = await annotatedDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var symbol = semanticModel.GetDeclaredSymbol(annotatedToken.Parent, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(annotatedSolution, symbol, newName, null, cancellationToken).ConfigureAwait(false);

            foreach (var project in newSolution.Projects)
            {
                var compilation = await project.GetCompilationAsync(cancellationToken);
                var diagnostics = compilation.GetDiagnostics();
                foreach (var diagnostic in diagnostics)
                {
                    if (NamingConflictDiagnosticIds.Contains(diagnostic.Id))
                    {
                        // If we got here, it means there was a naming conflict
                        // I believe every warning contains the name of the conflicting member in its description
                        // Therefore we can look whether the description contains the new identifier and if it does, return the annotated solution

                        if (diagnostic.GetMessage().Contains(newName))
                        {
                            return newSolution;
                        }
                    }
                }
            }

            // If we got here it means there weren't any new errors introduced by renaming
            // So we can just return the renamed solution without rename IDE helper
            var originalSemanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var originalSymbol = originalSemanticModel.GetDeclaredSymbol(declarationToken.Parent, cancellationToken);
            return await Renamer.RenameSymbolAsync(document.Project.Solution, originalSymbol, newName, null, cancellationToken);
        }
    }
}