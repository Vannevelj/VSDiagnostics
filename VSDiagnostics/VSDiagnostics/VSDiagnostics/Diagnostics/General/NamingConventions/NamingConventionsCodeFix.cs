using System;
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
                    x => RenameAsync(context.Document, identifier, root, diagnostic, context.CancellationToken), NamingConventionsAnalyzer.Rule.Id), diagnostic);
        }

        private async Task<Solution> RenameAsync(Document document, SyntaxToken identifier, SyntaxNode root, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var convention = diagnostic.Properties["convention"];
            var newIdentifier = identifier.WithConvention((NamingConvention) Enum.Parse(typeof(NamingConvention), convention));

            return await RenameHelper.RenameSymbolAsync(document, root, identifier, newIdentifier.Text, cancellationToken);
        }
    }
}