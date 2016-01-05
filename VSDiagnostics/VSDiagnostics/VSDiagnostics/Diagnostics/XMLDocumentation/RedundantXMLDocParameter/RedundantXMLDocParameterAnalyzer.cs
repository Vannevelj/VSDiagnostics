using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.XMLDocComments.RedundantXMLDocParameter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantXmlDocParameterAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.XmlDocCommentsCategory;
        private static readonly string Message = VSDiagnosticsResources.RedundantXmlDocParameterAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.RedundantXmlDocParameterAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.RedundantXmlDocParameter, Title, Message, Category, Severity, true);

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

            var xmlParamNodes = docNodes.SelectMany(n => n.Content.OfType<XmlElementSyntax>()).Where(e => e.StartTag.Name.LocalName.Text == "param");

            foreach (var node in xmlParamNodes)
            {
                var paramNames = method.ParameterList.Parameters.Select(p => p.Identifier.Text);
                var nodeParamName =
                    node.StartTag.Attributes.OfType<XmlNameAttributeSyntax>()
                        .First(a => a.Name.LocalName.Text == "name")
                        .Identifier.Identifier.Text;

                if (paramNames.Contains(nodeParamName))
                {
                    continue;
                }
                
                context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
            }
        }
    }
}