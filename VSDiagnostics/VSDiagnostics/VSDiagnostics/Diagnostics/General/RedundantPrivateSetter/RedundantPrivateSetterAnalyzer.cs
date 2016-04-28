using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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
            if (property == default(PropertyDeclarationSyntax))
            {
                return;
            }

            // We have to check whether or not the value is being written to
            // Since SymbolFinder does not work in an analyzer, we have to simulate finding the symbol ourselves
            // We can do this by getting the inner-most class declaration and then looking at all of its descendants
            // This is a fairly intensive operation but I'm not aware of any alternative
            var classDeclaration = setAccessor.Ancestors().OfType<ClassDeclarationSyntax>(SyntaxKind.ClassDeclaration).First();
            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
            var propertySymbol = context.SemanticModel.GetDeclaredSymbol(property);

            foreach (var partialDeclaration in classSymbol.DeclaringSyntaxReferences)
            {
                foreach (var descendant in partialDeclaration.GetSyntax().DescendantNodes())
                {
                    if (descendant.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    {
                        var assignment = (AssignmentExpressionSyntax) descendant;
                        ISymbol assignedSymbol;
                        if (descendant.SyntaxTree.Equals(property.SyntaxTree))
                        {
                            assignedSymbol = context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol;
                        }
                        else
                        {
                            var newModel = context.SemanticModel.Compilation.GetSemanticModel(partialDeclaration.SyntaxTree);
                            assignedSymbol = newModel.GetSymbolInfo(assignment.Left).Symbol;
                        }

                        if (assignedSymbol != null && assignedSymbol.Equals(propertySymbol))
                        {
                            var hasConstructorAncestor = false;
                            foreach (var ancestor in assignment.Ancestors())
                            {
                                if (ancestor.IsKind(SyntaxKind.ConstructorDeclaration))
                                {
                                    hasConstructorAncestor = true;
                                }
                            }

                            if (!hasConstructorAncestor)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, setAccessor.GetLocation(), property?.Identifier.ValueText));
        }
    }
}