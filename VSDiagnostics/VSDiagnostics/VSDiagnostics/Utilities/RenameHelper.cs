// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Found at https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/Helpers/RenameHelper.cs

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace VSDiagnostics.Utilities
{
    internal static class RenameHelper
    {
        private static readonly string[] NamingConflictDiagnosticIds = {"CS0102", "CS0128", "CS0101", "CS0542"};

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