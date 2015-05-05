using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace VSDiagnostics.Diagnostics.General.NullableToShorthand
{
    [ExportCodeFixProvider("NullableToShorthand", LanguageNames.CSharp), Shared]
    public class NullableToShorthandCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(NullableToShorthandAnalyzer.DiagnosticId);
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override Task ComputeFixesAsync(CodeFixContext context)
        {
            throw new NotImplementedException();
        }
    }
}