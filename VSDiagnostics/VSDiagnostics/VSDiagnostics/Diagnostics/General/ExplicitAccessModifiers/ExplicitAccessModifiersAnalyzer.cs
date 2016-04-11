using System.Collections.Immutable;
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
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ExplicitAccessModifiersAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ExplicitAccessModifiersAnalyzerTitle;

        private readonly SyntaxKind[] _accessModifierKinds =
        {
            SyntaxKind.PublicKeyword,
            SyntaxKind.ProtectedKeyword,
            SyntaxKind.InternalKeyword,
            SyntaxKind.PrivateKeyword
        };

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ExplicitAccessModifiers, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol,
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

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            if (context.Node.IsKind(SyntaxKind.ClassDeclaration))
            {
                var declarationExpression = (ClassDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.StructDeclaration))
            {
                var declarationExpression = (StructDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.EnumDeclaration))
            {
                var declarationExpression = (EnumDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.DelegateDeclaration))
            {
                var declarationExpression = (DelegateDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                var declarationExpression = (InterfaceDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.FieldDeclaration))
            {
                var declarationExpression = (FieldDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        "private"));
                }
            }

            if (context.Node.IsKind(SyntaxKind.PropertyDeclaration))
            {
                var declarationExpression = (PropertyDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                    declarationExpression.ExplicitInterfaceSpecifier == null)
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.MethodDeclaration))
            {
                var declarationExpression = (MethodDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                    !declarationExpression.Modifiers.Contains(SyntaxKind.PartialKeyword) &&
                    declarationExpression.ExplicitInterfaceSpecifier == null)
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.ConstructorDeclaration))
            {
                var declarationExpression = (ConstructorDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                    !declarationExpression.Modifiers.Contains(SyntaxKind.StaticKeyword))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.EventFieldDeclaration))
            {
                var declarationExpression = (EventFieldDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        "private"));
                }
            }

            if (context.Node.IsKind(SyntaxKind.EventDeclaration))
            {
                var declarationExpression = (EventDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }

            if (context.Node.IsKind(SyntaxKind.IndexerDeclaration))
            {
                var declarationExpression = (IndexerDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                    declarationExpression.ExplicitInterfaceSpecifier == null)
                {
                    var accessibility =
                        context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibility.ToString().ToLowerInvariant()));
                }
            }
        }
    }
}