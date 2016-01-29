using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ExplicitAccessModifiers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ExplicitAccessModifiersAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Info;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ExplicitAccessModifiersAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ExplicitAccessModifiersAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ExplicitAccessModifiers, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.StructDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent is InterfaceDeclarationSyntax)
            {
                return;
            }

            if (context.Node is ClassDeclarationSyntax)
            {
                var declarationExpression = (ClassDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is StructDeclarationSyntax)
            {
                var declarationExpression = (StructDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is EnumDeclarationSyntax)
            {
                var declarationExpression = (EnumDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is DelegateDeclarationSyntax)
            {
                var declarationExpression = (DelegateDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is InterfaceDeclarationSyntax)
            {
                var declarationExpression = (InterfaceDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is FieldDeclarationSyntax)
            {
                var declarationExpression = (FieldDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        "private"));
                }
            }

            if (context.Node is PropertyDeclarationSyntax)
            {
                var declarationExpression = (PropertyDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())) &&
                    declarationExpression.ExplicitInterfaceSpecifier == null)
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is MethodDeclarationSyntax)
            {
                var declarationExpression = (MethodDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())) &&
                    declarationExpression.Modifiers.All(m => m.Kind() != SyntaxKind.PartialKeyword) &&
                    declarationExpression.ExplicitInterfaceSpecifier == null)
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is ConstructorDeclarationSyntax)
            {
                var declarationExpression = (ConstructorDeclarationSyntax) context.Node;
                if (
                    !declarationExpression.Modifiers.Any(
                        m => _accessModifierKinds.Contains(m.Kind()) || m.Kind() == SyntaxKind.StaticKeyword))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is EventFieldDeclarationSyntax)
            {
                var declarationExpression = (EventFieldDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        "private"));
                }
            }

            if (context.Node is EventDeclarationSyntax)
            {
                var declarationExpression = (EventDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }

            if (context.Node is IndexerDeclarationSyntax)
            {
                var declarationExpression = (IndexerDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _accessModifierKinds.Contains(m.Kind())) &&
                     declarationExpression.ExplicitInterfaceSpecifier == null)
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLower()));
                }
            }
        }

        private readonly SyntaxKind[] _accessModifierKinds =
        {
            SyntaxKind.PublicKeyword,
            SyntaxKind.ProtectedKeyword,
            SyntaxKind.InternalKeyword,
            SyntaxKind.PrivateKeyword
        };
    }
}