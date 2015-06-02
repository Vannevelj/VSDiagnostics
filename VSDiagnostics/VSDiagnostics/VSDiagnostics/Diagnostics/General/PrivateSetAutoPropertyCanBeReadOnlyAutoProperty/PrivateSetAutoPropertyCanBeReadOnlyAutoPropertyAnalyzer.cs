using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;


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

        private List<SyntaxToken> _tokens = new List<SyntaxToken>();

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            Diagnostic diagnostic = null;

            var asProperty = context.Node as PropertyDeclarationSyntax;
            if (asProperty != null)
            {
                diagnostic = HandleProperty(asProperty);
            }

            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic HandleProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
    
            var setter = propertyDeclaration.AccessorList.Accessors.FirstOrDefault(s => s.IsKind(SyntaxKind.SetAccessorDeclaration) 
                                                                                    && s.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)));
            if (setter != null)
            {
                if ( !TreeHasMethodsThatSetProperty(propertyDeclaration) )
                {
                    return Diagnostic.Create(Rule, setter.GetLocation(), "Property Setter", propertyDeclaration.Identifier);
                }
            }

            return null;
        }

        private static bool TreeHasMethodsThatSetProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            var root = propertyDeclaration.SyntaxTree.GetRoot();
            var assignments = root.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                                
            return assignments.Select(a => a.Left)
                                .Cast<MemberAccessExpressionSyntax>()
                                .Select(l => l.Name.Identifier)
                                .Any
                                (
                                    i => i.Text == propertyDeclaration.Identifier.Text
                                    && i.Parent.Ancestors().All(anc => anc.GetType() != typeof(ConstructorDeclarationSyntax))
                                );
        }
    }
}
