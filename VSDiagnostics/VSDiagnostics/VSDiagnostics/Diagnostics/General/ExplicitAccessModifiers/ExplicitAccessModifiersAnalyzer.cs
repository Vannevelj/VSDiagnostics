using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.ExplicitAccessModifiers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class ExplicitAccessModifiersAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(ExplicitAccessModifiersAnalyzer);
        private const string Message = "Use explicit {0} modifier.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Info;
        private const string Title = "Use explicit {0} modifier.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.GetAccessorDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.StructDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax)
            {
                var declarationExpression = (ClassDeclarationSyntax) context.Node;
                if (!declarationExpression.Modifiers.Any(m => _modifierKinds.Contains(m.Kind())))
                {
                    var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                    var accessibilityKeyword = accessibility == Accessibility.Internal
                        ? SyntaxKind.InternalKeyword
                        : SyntaxKind.PrivateKeyword;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibilityKeyword));
                }
            }

            if (context.Node is StructDeclarationSyntax)
            {
                var declarationExpression = (StructDeclarationSyntax)context.Node;
                if (!declarationExpression.Modifiers.Any(m => _modifierKinds.Contains(m.Kind())))
                {
                    var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                    var accessibilityKeyword = accessibility == Accessibility.Internal
                        ? SyntaxKind.InternalKeyword
                        : SyntaxKind.PrivateKeyword;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibilityKeyword));
                }
            }

            if (context.Node is EnumDeclarationSyntax)
            {
                var declarationExpression = (EnumDeclarationSyntax)context.Node;
                if (!declarationExpression.Modifiers.Any(m => _modifierKinds.Contains(m.Kind())))
                {
                    var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                    var accessibilityKeyword = accessibility == Accessibility.Internal
                        ? SyntaxKind.InternalKeyword
                        : SyntaxKind.PrivateKeyword;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibilityKeyword));
                }
            }

            if (context.Node is InterfaceDeclarationSyntax)
            {
                var declarationExpression = (InterfaceDeclarationSyntax)context.Node;
                if (!declarationExpression.Modifiers.Any(m => _modifierKinds.Contains(m.Kind())))
                {
                    var accessibility = context.SemanticModel.GetDeclaredSymbol(declarationExpression).DeclaredAccessibility;
                    var accessibilityKeyword = accessibility == Accessibility.Internal
                        ? SyntaxKind.InternalKeyword
                        : SyntaxKind.PrivateKeyword;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, declarationExpression.GetLocation(),
                        accessibilityKeyword));
                }
            }
        }

        private readonly SyntaxKind[] _modifierKinds =
        {
            SyntaxKind.PublicKeyword,
            SyntaxKind.ProtectedKeyword,
            SyntaxKind.InternalKeyword,
            SyntaxKind.PrivateKeyword
        };
    }
}
