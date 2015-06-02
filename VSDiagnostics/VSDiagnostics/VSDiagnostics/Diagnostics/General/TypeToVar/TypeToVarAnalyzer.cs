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
        public const string DiagnosticId = nameof(TypeToVarAnalyzer);
        internal const string Title = "Use var instead of type.";
        internal const string Message = "Actual type can be replaced with 'var'.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = context.Node as LocalDeclarationStatementSyntax;
            if (localDeclaration == null)
            {
                return;
            }

            if (localDeclaration.Declaration == null)
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
            if (variable == null || variable.Initializer == null)
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