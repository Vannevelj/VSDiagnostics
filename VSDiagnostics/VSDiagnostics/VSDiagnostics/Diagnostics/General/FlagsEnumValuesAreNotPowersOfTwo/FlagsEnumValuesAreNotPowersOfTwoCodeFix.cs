using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.FlagsEnumValuesAreNotPowersOfTwo
{
    [ExportCodeFixProvider("FlagsEnumValuesAreNotPowersOfTwo", LanguageNames.CSharp), Shared]
    public class FlagsEnumValuesAreNotPowersOfTwoCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(CodeAction.Create(VSDiagnosticsResources.FlagsEnumValuesAreNotPowersOfTwoCodeFixTitle, x => AdjustEnumValues(context.Document, root, statement), nameof(FlagsEnumValuesAreNotPowersOfTwoAnalyzer)), diagnostic);
        }

        private async Task<Solution> AdjustEnumValues(Document document, SyntaxNode root, SyntaxNode statement)
        {
            var semanticModel = await document.GetSemanticModelAsync();

            var declarationExpression = statement as EnumDeclarationSyntax;

            var declaredSymbol = semanticModel.GetDeclaredSymbol(declarationExpression);
            var typeName = declaredSymbol.EnumUnderlyingType.MetadataName;

            var enumMemberDeclarations = declarationExpression.ChildNodes().OfType<EnumMemberDeclarationSyntax>().ToList();

            for (var i = 0; i < enumMemberDeclarations.Count; i++)
            {
                SyntaxToken literalToken;

                switch (typeName)
                {
                    case "Int16":
                        var newShort = i == 0 ? (short)0 : (short)Math.Pow(2, i - 1);
                        literalToken = SyntaxFactory.Literal(newShort);
                        break;
                    case "UInt16":
                        var newUshort = i == 0 ? (ushort)0 : (ushort)Math.Pow(2, i - 1);
                        literalToken = SyntaxFactory.Literal(newUshort);
                        break;
                    case "Int32":
                        var newInt = i == 0 ? 0 : (int)Math.Pow(2, i - 1);
                        literalToken = SyntaxFactory.Literal(newInt);
                        break;
                    case "UInt32":
                        var newUint = i == 0 ? 0 : (uint)Math.Pow(2, i - 1);
                        literalToken = SyntaxFactory.Literal(newUint);
                        break;
                    case "Int64":
                        var newLong = i == 0 ? 0 : (long)Math.Pow(2, i - 1);
                        literalToken = SyntaxFactory.Literal(newLong);
                        break;
                    default:
                        var newUlong = i == 0 ? 0 : (ulong)Math.Pow(2, i - 1);
                        literalToken = SyntaxFactory.Literal(newUlong);
                        break;
                }

                var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    literalToken);

                var newEqualsValue = SyntaxFactory.EqualsValueClause(literalExpression);
                enumMemberDeclarations[i] = enumMemberDeclarations[i].WithEqualsValue(newEqualsValue);
            }

            var newStatement = declarationExpression.WithMembers(SyntaxFactory.SeparatedList(enumMemberDeclarations));

            var newRoot = root.ReplaceNode(statement, newStatement);
            return document.WithSyntaxRoot(newRoot).Project.Solution;
        }
    }
}