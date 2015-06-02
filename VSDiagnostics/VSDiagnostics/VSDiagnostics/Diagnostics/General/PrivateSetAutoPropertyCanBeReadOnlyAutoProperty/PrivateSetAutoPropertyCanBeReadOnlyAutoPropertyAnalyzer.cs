using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace VSDiagnostics.Diagnostics.General.PrivateSetAutoPropertyCanBeReadOnlyAutoProperty
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PrivateSetAutoPropertyCanBeReadOnlyAutoPropertyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(PrivateSetAutoPropertyCanBeReadOnlyAutoPropertyAnalyzer);
        internal const string Title = "Private Set AutoProperty is only set once. It can be made readonly.";
        internal const string Message = "Private Set can be made ReadOnly";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.PropertyDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            Diagnostic diagnostic = null;

            var asProperty = context.Node as PropertyDeclarationSyntax;
            if (asProperty != null)
            {
                diagnostic = HandleProperty(asProperty);
            }

            var asConstructor = context.Node as ConstructorDeclarationSyntax;
            if (asConstructor != null)
            {
                diagnostic = HandleConstructor(asConstructor);
            }

            //var asMethod = context.Node as MethodDeclarationSyntax;
            //if (asMethod != null)
            //{
            //    diagnostic = HandleMethod(asMethod);
            //}

            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic HandleConstructor(ConstructorDeclarationSyntax asConstructor)
        {
            // throw new NotImplementedException();
            return null;
        }

        private Diagnostic HandleProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.ExpressionBody != null)
            {
                return null;
            }

            var setter = propertyDeclaration.AccessorList.Accessors;

            return null;
        }
    }
}
