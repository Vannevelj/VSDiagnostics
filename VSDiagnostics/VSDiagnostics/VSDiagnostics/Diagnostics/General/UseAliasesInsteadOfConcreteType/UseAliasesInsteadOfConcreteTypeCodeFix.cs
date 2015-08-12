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
    [ExportCodeFixProvider("UseAliasesInsteadOfConcreteType", LanguageNames.CSharp), Shared]
    public class UseAliasesInsteadOfConcreteTypeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.UseAliasesInsteadOfConcreteTypeCodeFixTitle, x => AsToCastAsync(context.Document, root, statement), nameof(UseAliasesInsteadOfConcreteTypeAnalyzer)), diagnostic);
        }

        private async Task<Solution> AsToCastAsync(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var semanticModel = await document.GetSemanticModelAsync();
            string typeName;

            if (statement is IdentifierNameSyntax)
            {
                typeName = semanticModel.GetSymbolInfo((IdentifierNameSyntax)statement).Symbol.MetadataName;
            }
            else
            {
                typeName = semanticModel.GetSymbolInfo((QualifiedNameSyntax)statement).Symbol.MetadataName;
            }

            var aliasToken = MapConcreteTypeToPredefinedTypeAlias.First(kvp => kvp.Key == typeName).Value;

            var newExpression = SyntaxFactory.PredefinedType(SyntaxFactory.Token(aliasToken));
            var newRoot = root.ReplaceNode(statement, newExpression).WithAdditionalAnnotations(Formatter.Annotation);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument.Project.Solution;
        }

        public static readonly Dictionary<string, SyntaxKind> MapConcreteTypeToPredefinedTypeAlias =
            new Dictionary<string, SyntaxKind>
            {
                {"Int16", SyntaxKind.ShortKeyword},
                {"Int32", SyntaxKind.IntKeyword},
                {"Int64", SyntaxKind.LongKeyword},
                {"UInt16", SyntaxKind.UShortKeyword},
                {"UInt32", SyntaxKind.UIntKeyword},
                {"UInt64", SyntaxKind.ULongKeyword},
                {"Object", SyntaxKind.ObjectKeyword},
                {"Byte", SyntaxKind.ByteKeyword},
                {"SByte", SyntaxKind.SByteKeyword},
                {"Char", SyntaxKind.CharKeyword},
                {"Boolean", SyntaxKind.BoolKeyword},
                {"Single", SyntaxKind.FloatKeyword},
                {"Double", SyntaxKind.DoubleKeyword},
                {"Decimal", SyntaxKind.DecimalKeyword},
                {"String", SyntaxKind.StringKeyword}
            };
    }
}