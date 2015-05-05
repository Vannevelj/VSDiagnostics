using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.NullableToShorthand
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableToShorthandAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(NullableToShorthandAnalyzer);
        internal const string Title = "Use the shorthand T? notation for a nullable type.";
        internal const string Message = "{0} can be written using the shorthand nullable notation.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        private SyntaxNodeAnalysisContext _context;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol,
                SyntaxKind.LocalDeclarationStatement,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.Parameter,
                SyntaxKind.TypeParameter,
                SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            _context = context;
            switch (context.Node.CSharpKind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    HandleLocalDeclaration(context.Node as LocalDeclarationStatementSyntax);
                    break;
                case SyntaxKind.FieldDeclaration:
                    HandleFieldDeclaration(context.Node as FieldDeclarationSyntax);
                    break;
                case SyntaxKind.Parameter:
                    HandleParameterDeclaration(context.Node as ParameterSyntax);
                    break;
                case SyntaxKind.TypeParameter:
                    HandleTypeParameterDeclaration(context.Node as TypeParameterSyntax);
                    break;
                case SyntaxKind.PropertyDeclaration:
                    HandlePropertyDeclaration(context.Node as PropertyDeclarationSyntax);
                    break;
                default:
                    return;
            }
        }

        private void HandlePropertyDeclaration(PropertyDeclarationSyntax property)
        {
            throw new NotImplementedException();
        }

        private void HandleTypeParameterDeclaration(TypeParameterSyntax type)
        {
            throw new NotImplementedException();
        }

        private void HandleParameterDeclaration(ParameterSyntax parameter)
        {
            throw new NotImplementedException();
        }

        private void HandleFieldDeclaration(FieldDeclarationSyntax field)
        {
            throw new NotImplementedException();
        }

        private void HandleLocalDeclaration(LocalDeclarationStatementSyntax local)
        {
            var declaredType = local.Declaration.Type;
            if (!declaredType.IsKind(SyntaxKind.NullableType))
            {
                return;
            }

            // Type is already in T? notation
            var nullableType = declaredType as NullableTypeSyntax;
            if (nullableType == null)
            {
                return;
            }

            _context.ReportDiagnostic(Diagnostic.Create(Rule, local.GetLocation(), local.Declaration.Variables.First().Identifier.Text));
        }
    }
}