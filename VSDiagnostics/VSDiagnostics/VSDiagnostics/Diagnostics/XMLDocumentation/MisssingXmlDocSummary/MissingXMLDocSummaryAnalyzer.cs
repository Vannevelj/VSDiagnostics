using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.XMLDocumentation.MisssingXmlDocSummary
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingXmlDocSummaryAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.XmlDocCommentsCategory;
        private static readonly string Message = VSDiagnosticsResources.MisssingXmlDocSummaryAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.MisssingXmlDocSummaryAnalyzerTitle;

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

            var returnType = method.ReturnType as PredefinedTypeSyntax;
            if (returnType?.Keyword.Text == "void")
            {
                return;
            }

            var docNodes = method.GetLeadingTrivia()
                    .Where(n => n.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    .Select(t => t.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .ToList();

            if (!docNodes.Any())
            {
                return;
            }

            if (docNodes.SelectMany(n => n.Content.OfType<XmlElementSyntax>())
                .All(e => e.StartTag.Name.LocalName.Text != "summary"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation()));
            }
        }
    }
}