using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonEncapsulatedOrMutableFieldAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.NonEncapsulatedOrMutableFieldAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.NonEncapsulatedOrMutableFieldAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.NonEncapsulatedOrMutableField, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax) context.Node;

            // Don't handle (semi-)immutable fields
            if (fieldDeclaration.Modifiers.Any(
                    x => x.IsKind(SyntaxKind.ConstKeyword) || x.IsKind(SyntaxKind.ReadOnlyKeyword)))
            {
                return;
            }

            // Only handle public, internal and protected internal fields
            if (!fieldDeclaration.Modifiers.Any(
                    x => x.IsKind(SyntaxKind.PublicKeyword) || x.IsKind(SyntaxKind.InternalKeyword)))
            {
                return;
            }

            // Fields passed as out or ref can't be made properties since that would introduce a compilation error
            // However finding all references to a given symbol from an analyzer's context is hard to accomplish (if not impossible)
            // We have to wait for SymbolFinder.FindReferencesAsync() to be applicable here (see https://github.com/dotnet/roslyn/issues/3394)
            // In the meantime we apply an approach that just checks the current syntax tree and sees if any of the references there are passed by ref or out
            // This should account for a large amount of usages but not all e.g.
            //
            // class X
            // {
            //     public int intX;
            // }
            //
            // class Y
            // {
            //    private X x;
            //
            //    public Y()
            //    {
            //        Method(out x.intX);
            //    }
            //
            //    void Method(out int par)
            //    {
            //        par = 5;
            //    }
            // }
            //
            // In this scenario our analyzer is triggered on the field in class X and won't find any references inside that syntax tree (assuming separate files)
            // However it won't find any ref/out usages there so it will create the diagnostic -- which obviously isn't supposed to happen because the other syntax tree DOES contain that
            // However until this ability is added, we'll just have to live with it
            var outerClass = context.Node.Ancestors().OfType<ClassDeclarationSyntax>().LastOrDefault();
            if (outerClass != null)
            {
                var semanticModel = context.SemanticModel;

                foreach (var variable in fieldDeclaration.Declaration.Variables)
                {
                    var fieldSymbol = semanticModel.GetDeclaredSymbol(variable);

                    foreach (var descendant in outerClass.DescendantNodes().OfType<IdentifierNameSyntax>())
                    {
                        var descendentSymbol = semanticModel.GetSymbolInfo(descendant).Symbol;
                        if (descendentSymbol != null && descendentSymbol.Equals(fieldSymbol))
                        {
                            // The field is being referenced
                            // Next we check whether it is referenced as an argument and passed by ref/out
                            var argument = descendant.AncestorsAndSelf().OfType<ArgumentSyntax>().FirstOrDefault();
                            if (argument != null && !argument.RefOrOutKeyword.IsMissing)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                // Using .Text instead of .ValueText so verbatim and unicode-escaped identifiers would display as such rather than having it stripped out.
                context.ReportDiagnostic(Diagnostic.Create(Rule, variable.Identifier.GetLocation(),
                    variable.Identifier.Text));
            }
        }
    }
}