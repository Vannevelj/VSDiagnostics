using System.Collections.Immutable;
using System.Linq;
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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.GenericName);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var argumentList = (GenericNameSyntax) context.Node;
            var identifier = "Unnamed variable";
            var ancestorNodes = new[] { SyntaxKind.LocalDeclarationStatement, SyntaxKind.FieldDeclaration, SyntaxKind.Parameter, SyntaxKind.PropertyDeclaration };
            var parentNode = context.Node.AncestorsAndSelf().FirstOrDefault(x => ancestorNodes.Contains(x.CSharpKind()));

            if (parentNode != null)
            {
                switch (parentNode.CSharpKind())
                {
                    case SyntaxKind.LocalDeclarationStatement:
                    {
                        identifier = ((LocalDeclarationStatementSyntax) parentNode).Declaration?.Variables.FirstOrDefault()?.Identifier.Text;
                        break;
                    }
                    case SyntaxKind.FieldDeclaration:
                    {
                        identifier = ((FieldDeclarationSyntax) parentNode).Declaration?.Variables.FirstOrDefault()?.Identifier.Text;
                        break;
                    }
                    case SyntaxKind.Parameter:
                    {
                        identifier = ((ParameterSyntax) parentNode).Identifier.Text;
                        break;
                    }
                    case SyntaxKind.TypeParameter:
                    {
                        identifier = ((TypeParameterSyntax) parentNode).Identifier.Text;
                        break;
                    }
                    case SyntaxKind.PropertyDeclaration:
                    {
                        identifier = ((PropertyDeclarationSyntax) parentNode).Identifier.Text;
                        break;
                    }
                }
            }


            Handle(identifier, context.Node.GetLocation(), argumentList, context);
        }

        private void Handle(string identifier, Location location, GenericNameSyntax genericName, SyntaxNodeAnalysisContext context)
        {
            // Leave if type is in nullable form
            if (genericName.IsKind(SyntaxKind.NullableType))
            {
                return;
            }

            var genericType = context.SemanticModel.GetTypeInfo(genericName);
            if (genericType.Type?.MetadataName == "Nullable`1")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, location, identifier));
            }
        }
    }
}