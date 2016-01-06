using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.XMLDocumentation.MissingXMLDocParameter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingXmlDocParameterAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.XmlDocCommentsCategory;
        private static readonly string Message = VSDiagnosticsResources.MissingXmlDocParameterAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.MissingXmlDocParameterAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.MissingXmlDocParameter, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }
        
        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;
            
            if (method == null)
            {
                return; 
            }

            var docNodes =
                method.GetLeadingTrivia()
                    .Where(n => n.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    .Select(t => t.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .ToList();

            if (!docNodes.Any())
            {
                return;
            }

            var paramNames = method.ParameterList.Parameters.Select(p => p.Identifier.Text).ToList();
            var xmlParamNodes = docNodes.SelectMany(n => n.Content.OfType<XmlElementSyntax>()).Where(e => e.StartTag.Name.LocalName.Text == "param").ToList();

            var xmlParamNodeNames =
                xmlParamNodes.SelectMany(n =>
                        n.StartTag.Attributes.OfType<XmlNameAttributeSyntax>()
                            .Where(a => a.Name.LocalName.Text == "name")
                            .Select(t => t.Identifier.Identifier.Text)).ToList();

            if (!paramNames.All(n => xmlParamNodeNames.Contains(n)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation()));
            }
        }
    }
}