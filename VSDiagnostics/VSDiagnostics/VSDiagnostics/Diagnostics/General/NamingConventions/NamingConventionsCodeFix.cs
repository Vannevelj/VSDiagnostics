using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NamingConventions
{
    [ExportCodeFixProvider(nameof(NamingConventionsCodeFix), LanguageNames.CSharp), Shared]
    public class NamingConventionsCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NamingConventionsAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var identifier = root.FindToken(diagnosticSpan.Start);
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.NamingConventionsCodeFixTitle,
                    x => RenameAsync(context.Document, identifier, root), NamingConventionsAnalyzer.Rule.Id), diagnostic);
        }

        internal async Task<Solution> RenameAsync(Document document, SyntaxToken identifier, SyntaxNode root)
        {
            var identifierParent = identifier.Parent;
            var newIdentifier = default(SyntaxToken);

            do
            {
                var parentAsField = identifierParent as FieldDeclarationSyntax;
                if (parentAsField != null)
                {
                    if (
                        parentAsField.Modifiers.Any(
                            x =>
                                new[]
                                {SyntaxKind.InternalKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PublicKeyword}.Any(
                                    keyword => x.IsKind(keyword))))
                    {
                        newIdentifier = identifier.WithConvention(NamingConvention.UpperCamelCase);
                    }
                    else
                    {
                        newIdentifier = identifier.WithConvention(NamingConvention.UnderscoreLowerCamelCase);
                    }
                    break;
                }

                if (identifierParent is PropertyDeclarationSyntax ||
                    identifierParent is MethodDeclarationSyntax ||
                    identifierParent is ClassDeclarationSyntax ||
                    identifierParent is StructDeclarationSyntax ||
                    identifierParent is EnumDeclarationSyntax ||
                    identifierParent is EnumMemberDeclarationSyntax)
                {
                    newIdentifier = identifier.WithConvention(NamingConvention.UpperCamelCase);
                    break;
                }

                if (identifierParent is LocalDeclarationStatementSyntax || identifierParent is ParameterSyntax)
                {
                    newIdentifier = identifier.WithConvention(NamingConvention.LowerCamelCase);
                    break;
                }

                if (identifierParent is InterfaceDeclarationSyntax)
                {
                    newIdentifier = identifier.WithConvention(NamingConvention.InterfacePrefixUpperCamelCase);
                    break;
                }

                identifierParent = identifierParent.Parent;
            } while (identifierParent != null);

            //var newRoot = root.ReplaceToken(identifier, identifier.WithAdditionalAnnotations(RenameAnnotation.Create()));
            //var newDocument = document.WithSyntaxRoot(newRoot);
            var semanticModel = await newDocument.GetSemanticModelAsync();
            var symbol = semanticModel.GetDeclaredSymbol(identifier.Parent);
            var solution = newDocument.Project.Solution;
            var options = solution.Workspace.Options;

            return await Renamer.RenameSymbolAsync(solution, symbol, newIdentifier.Text, options);
        }
    }
}