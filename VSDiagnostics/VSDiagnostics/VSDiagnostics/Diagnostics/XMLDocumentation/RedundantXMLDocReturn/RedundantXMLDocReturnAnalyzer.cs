using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.XMLDocumentation.RedundantXMLDocReturn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantXmlDocReturnAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.XmlDocCommentsCategory;
        private static readonly string Message = VSDiagnosticsResources.RedundantXmlDocReturnAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.RedundantXmlDocReturnAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.RedundantXmlDocReturn, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;

            var returnType = method?.ReturnType as PredefinedTypeSyntax;
            if (returnType == null || !returnType.Keyword.IsKind(SyntaxKind.VoidKeyword))
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

            foreach (var node in docNodes.Where(node => node.Content.OfType<XmlElementSyntax>().Any(n => n.StartTag.Name.LocalName.Text == "returns")))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
            }
        }
    }
}