using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FindSymbols;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.RedundantPrivateSetter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantPrivateSetterAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.RedundantPrivateSetterAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.RedundantPrivateSetterAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.RedundantPrivateSetter, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SetAccessorDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var setAccessor = (AccessorDeclarationSyntax) context.Node;

            // Since there are no modifiers that can work with 'private', we can just assume that there should be one modifier: private
            var hasOneKeyword = setAccessor.Modifiers.Count == 1;
            var hasPrivateKeyword = setAccessor.Modifiers[0].IsKind(SyntaxKind.PrivateKeyword);
            if (!(hasOneKeyword && hasPrivateKeyword))
            {
                return;
            }

            var property = default(PropertyDeclarationSyntax);
            foreach (var ancestor in context.Node.Ancestors())
            {
                if (ancestor.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    property = (PropertyDeclarationSyntax) ancestor;
                }
            }

            var classDeclaration = setAccessor.Ancestors().OfType<ClassDeclarationSyntax>(SyntaxKind.ClassDeclaration);


            context.ReportDiagnostic(Diagnostic.Create(Rule, setAccessor.GetLocation(), property?.Identifier.ValueText));
        }
    }
}