using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ElementaryMethodsOfTypeInCollectionNotOverridden
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.ElementaryMethodsOfTypeInCollectionNotOverridden, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var objectTypeInfo = context.SemanticModel.GetTypeInfo(context.Node).Type;

            var ienumerableIsImplemented = false;
            foreach (var @interface in objectTypeInfo.AllInterfaces)
            {
                if (@interface.ToDisplayString() == typeof (IEnumerable).FullName)
                {
                    ienumerableIsImplemented = true;
                    break;
                }
            }

            if (!ienumerableIsImplemented)
            {
                return;
            }

            var objectType = ((ObjectCreationExpressionSyntax) context.Node).Type as GenericNameSyntax;
            var genericType = objectType?.TypeArgumentList.Arguments.FirstOrDefault();

            if (genericType == null)
            {
                return;
            }

            var genericTypeInfo = context.SemanticModel.GetTypeInfo(genericType).Type;

            if (genericTypeInfo == null)
            {
                return;
            }

            var implementsEquals = false;
            var implementsGetHashCode = false;
            foreach (var member in genericTypeInfo.GetMembers())
            {
                if (member.Name == nameof(Equals))
                {
                    implementsEquals = true;
                }

                if (member.Name == nameof(GetHashCode))
                {
                    implementsGetHashCode = true;
                }
            }

            if (!implementsEquals || !implementsGetHashCode)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, genericType.GetLocation()));
            }
        }
    }
}