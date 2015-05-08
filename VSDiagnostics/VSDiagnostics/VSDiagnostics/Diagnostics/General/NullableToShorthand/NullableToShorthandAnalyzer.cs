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
        private SyntaxNodeAnalysisContext _context;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol,
                SyntaxKind.LocalDeclarationStatement,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.Parameter,
                SyntaxKind.TypeParameter,
                SyntaxKind.TypeArgumentList,
                SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            _context = context;
            switch (context.Node.CSharpKind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                {
                    var obj = (LocalDeclarationStatementSyntax) context.Node;
                    var identifier = obj.Declaration?.Variables.FirstOrDefault()?.Identifier.Text;
                    var location = obj.GetLocation();
                    Handle(identifier, location, obj.Declaration.Type);
                    break;
                }

                case SyntaxKind.FieldDeclaration:
                {
                    var obj = (FieldDeclarationSyntax) context.Node;
                    var identifier = obj.Declaration.Variables.First().Identifier.Text;
                    var location = obj.GetLocation();
                    Handle(identifier, location, obj.Declaration.Type);
                    break;
                }
                case SyntaxKind.Parameter:
                {
                    var obj = (ParameterSyntax) context.Node;
                    var identifier = obj.Identifier.Text;
                    var location = obj.GetLocation();
                    Handle(identifier, location, obj.Type);
                    break;
                }
                case SyntaxKind.TypeArgumentList:
                {
                    var obj = (TypeArgumentListSyntax) context.Node;
                    var identifier = obj.Ancestors()?.OfType<VariableDeclarationSyntax>()?.FirstOrDefault()?.Variables.FirstOrDefault()?.Identifier.Text;
                    var location = obj.GetLocation();
                    Handle(identifier, location, obj.Arguments.ToArray());
                    break;
                }
                case SyntaxKind.PropertyDeclaration:
                {
                    var obj = (PropertyDeclarationSyntax) context.Node;
                    var identifier = obj.Identifier.Text;
                    var location = obj.GetLocation();
                    Handle(identifier, location, obj.Type);
                    break;
                }

                default:
                    return;
            }
        }

        private void Handle(string identifier, Location location, params TypeSyntax[] types)
        {
            foreach (var type in types)
            {
                // Leave if type is in nullable form
                if (type.IsKind(SyntaxKind.NullableType))
                {
                    return;
                }

                // Leave if type is not a generic
                var genericType = type as GenericNameSyntax;
                if (genericType == null)
                {
                    return;
                }

                // Display diagnostic if argument is of type Nullable
                var innerType = _context.SemanticModel.GetTypeInfo(type);
                if (innerType.Type.MetadataName == "Nullable`1")
                {
                    _context.ReportDiagnostic(Diagnostic.Create(Rule, location, identifier ?? "Unnamed variable"));
                    return;
                }

                foreach (var argument in genericType.TypeArgumentList.Arguments)
                {
                    Handle(identifier, location, argument);
                }
            }
        }
    }
}