using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
// ReSharper disable LoopCanBeConvertedToQuery

namespace VSDiagnostics.Diagnostics.General.SingleEmptyConstructor
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SingleEmptyConstructorAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.SingleEmptyConstructorAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.SingleEmptyConstructorAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.SingleEmptyConstructor, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ConstructorDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax) context.Node;

            // ctor must be public
            if (!constructorDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return;
            }

            // ctor must not have parameters
            if (constructorDeclaration.ParameterList.Parameters.Any())
            {
                return;
            }

            // ctor must have no body statements
            if (constructorDeclaration.Body == null || constructorDeclaration.Body.Statements.Any())
            {
                return;
            }

            // ctor must not contain comments
            foreach (var trivia in constructorDeclaration.Body.CloseBraceToken.LeadingTrivia)
            {
                if (trivia.IsCommentTrivia())
                {
                    return;
                }
            }

            // ctor must not have attributes
            if (constructorDeclaration.AttributeLists.NonLinqAny())
            {
                return;
            }

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(constructorDeclaration.Parent) as INamedTypeSymbol;
            if (classSymbol != null && classSymbol.Constructors.Length != 1)
            {
                return;
            }

            foreach (var trivia in constructorDeclaration.GetLeadingTrivia())
            {
                if (trivia.IsCommentTrivia())
                {
                    return;
                }
            }
            
            foreach (var node in constructorDeclaration.ChildNodes())
            {
                var nodeAsConstructorInitializerSyntax = node as ConstructorInitializerSyntax;
                if (nodeAsConstructorInitializerSyntax != null)
                {
                    var constructorInitializer = nodeAsConstructorInitializerSyntax;

                    // we must return false (to avoid the parent if) only if it is the base keyword
                    // and there are no arguments.
                    if (!constructorInitializer.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseKeyword) ||
                        constructorInitializer.ArgumentList.Arguments.NonLinqAny())
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, constructorDeclaration.GetLocation(),
                constructorDeclaration.Identifier));
        }
    }
}