using System;
using System.Collections.Generic;
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

namespace VSDiagnostics.Diagnostics.General.UseAliasesInsteadOfConcreteType
{
    [ExportCodeFixProvider(nameof(UseAliasesInsteadOfConcreteTypeCodeFix), LanguageNames.CSharp), Shared]
    public class UseAliasesInsteadOfConcreteTypeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeCodeFixTitle,
                    x => UseAliasAsync(context.Document, root, statement),
                    UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.Id), diagnostic);
        }

        private async Task<Solution> UseAliasAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var semanticModel = await document.GetSemanticModelAsync();
            var typeName = semanticModel.GetSymbolInfo(statement).Symbol.MetadataName;
            var aliasToken = MapConcreteTypeToPredefinedTypeAlias[typeName];

            var newExpression = SyntaxFactory.PredefinedType(SyntaxFactory.Token(aliasToken));
            var newRoot = root.ReplaceNode(statement, newExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument.Project.Solution;
        }

        private static readonly Dictionary<string, SyntaxKind> MapConcreteTypeToPredefinedTypeAlias =
            new Dictionary<string, SyntaxKind>
            {
                {nameof(Int16), SyntaxKind.ShortKeyword},
                {nameof(Int32), SyntaxKind.IntKeyword},
                {nameof(Int64), SyntaxKind.LongKeyword},
                {nameof(UInt16), SyntaxKind.UShortKeyword},
                {nameof(UInt32), SyntaxKind.UIntKeyword},
                {nameof(UInt64), SyntaxKind.ULongKeyword},
                {nameof(Object), SyntaxKind.ObjectKeyword},
                {nameof(Byte), SyntaxKind.ByteKeyword},
                {nameof(SByte), SyntaxKind.SByteKeyword},
                {nameof(Char), SyntaxKind.CharKeyword},
                {nameof(Boolean), SyntaxKind.BoolKeyword},
                {nameof(Single), SyntaxKind.FloatKeyword},
                {nameof(Double), SyntaxKind.DoubleKeyword},
                {nameof(Decimal), SyntaxKind.DecimalKeyword},
                {nameof(String), SyntaxKind.StringKeyword}
            };
    }
}