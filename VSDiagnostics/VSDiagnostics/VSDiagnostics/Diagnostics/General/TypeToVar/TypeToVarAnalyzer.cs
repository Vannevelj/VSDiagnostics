using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.TypeToVar
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TypeToVarAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.TypeToVarAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.TypeToVarAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.TypeToVar, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax) context.Node;

            if (localDeclaration.Declaration == null)
            {
                return;
            }

            var declaredType = localDeclaration.Declaration.Type;
            if (declaredType.IsVar)
            {
                return;
            }

            if (localDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword)))
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