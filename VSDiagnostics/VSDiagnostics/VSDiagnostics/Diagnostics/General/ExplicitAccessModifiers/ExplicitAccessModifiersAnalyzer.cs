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

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.ExplicitAccessModifiers, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleClass, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleConstructor, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(HandleDelegate, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(HandleEnum, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(HandleEvent, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(HandleEventField, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(HandleField, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(HandleIndexer, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(HandleInterface, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandleProperty, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(HandleStruct, SyntaxKind.StructDeclaration);
        }

        private void HandleClass(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (ClassDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleStruct(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (StructDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleEnum(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (EnumDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleDelegate(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (DelegateDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleInterface(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (InterfaceDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleField(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (FieldDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), "private"));
            }
        }

        private void HandleProperty(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            var declarationExpression = (PropertyDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                declarationExpression.ExplicitInterfaceSpecifier == null)
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleMethod(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            var declarationExpression = (MethodDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                !declarationExpression.Modifiers.Contains(SyntaxKind.PartialKeyword) &&
                declarationExpression.ExplicitInterfaceSpecifier == null)
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleConstructor(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (ConstructorDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                !declarationExpression.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleEventField(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            var declarationExpression = (EventFieldDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(), "private"));
            }
        }

        private void HandleEvent(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            var declarationExpression = (EventDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds))
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }

        private void HandleIndexer(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            var declarationExpression = (IndexerDeclarationSyntax) context.Node;
            if (!declarationExpression.Modifiers.ContainsAny(_accessModifierKinds) &&
                declarationExpression.ExplicitInterfaceSpecifier == null)
            {
                var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                    accessibility.ToString().ToLowerInvariant()));
            }
        }
    }
}