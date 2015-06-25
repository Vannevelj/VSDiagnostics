using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VSDiagnostics.Diagnostics.General.TypeToVar
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TypeToVarAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(TypeToVarAnalyzer);
        private const string Message = "Actual type can be replaced with 'var'.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Use var instead of type.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = context.Node as LocalDeclarationStatementSyntax;

            if (localDeclaration?.Declaration == null)
            {
                return;
            }

            var declaredType = localDeclaration.Declaration.Type;
            if (declaredType.IsVar)
            {
                return;
            }

            // can't have more than one implicitly-typed variable in a statement
            var variable = localDeclaration.Declaration.Variables.FirstOrDefault();
            if (variable?.Initializer == null)
            {
                return;
            }

            var variableType = context.SemanticModel.GetTypeInfo(declaredType).Type;
            var initializerType = context.SemanticModel.GetTypeInfo(variable.Initializer.Value).Type;

            if (Equals(variableType, initializerType))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, declaredType.GetLocation()));
            }
        }
    }
}