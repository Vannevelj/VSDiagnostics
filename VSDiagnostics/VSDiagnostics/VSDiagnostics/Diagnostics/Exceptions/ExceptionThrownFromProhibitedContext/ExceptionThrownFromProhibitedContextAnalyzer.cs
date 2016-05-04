using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Exceptions.ExceptionThrownFromProhibitedContext
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionThrownFromProhibitedContextAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.ExceptionsCategory;

        private static DiagnosticDescriptor ImplicitOperatorRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromImplicitOperator,
                VSDiagnosticsResources.ExceptionThrownFromImplicitOperatorAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromImplicitOperatorAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor PropertyGetterRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromPropertyGetter,
                VSDiagnosticsResources.ExceptionThrownFromPropertyGetterAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromPropertyGetterAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor StaticConstructorRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromStaticConstructor,
                VSDiagnosticsResources.ExceptionThrownFromStaticConstructorAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromStaticConstructorAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor FinallyBlockRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromFinallyBlock,
                VSDiagnosticsResources.ExceptionThrownFromFinallyBlockAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromFinallyBlockAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor EqualityOperatorRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromEqualityOperator,
                VSDiagnosticsResources.ExceptionThrownFromEqualityOperatorAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromEqualityOperatorAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor DisposeRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromDispose,
                VSDiagnosticsResources.ExceptionThrownFromDisposeAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromDisposeAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor FinalizerRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromFinalizer,
                VSDiagnosticsResources.ExceptionThrownFromFinalizerAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromFinalizerAnalyzerMessage, Category, Severity, true);

        private static DiagnosticDescriptor GetHashCodeRule
            => new DiagnosticDescriptor(DiagnosticId.ExceptionThrownFromGetHashCode,
                VSDiagnosticsResources.ExceptionThrownFromGetHashCodeAnalyzerTitle,
                VSDiagnosticsResources.ExceptionThrownFromGetHashCodeAnalyzerMessage, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(
                ImplicitOperatorRule,
                PropertyGetterRule,
                StaticConstructorRule,
                FinallyBlockRule,
                EqualityOperatorRule,
                DisposeRule,
                FinalizerRule,
                GetHashCodeRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ThrowStatement);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // Since the current node is a throw statement there is no symbol to be found. 
            // Therefore we look at whatever member is holding the statement (constructor, method, property, etc) and see what encloses that
            var containingType = context.SemanticModel.GetEnclosingSymbol(context.Node.SpanStart).ContainingType;
            var warningLocation = context.Node.GetLocation();

            foreach (var ancestor in context.Node.Ancestors())
            {
                if (ancestor.IsKind(SyntaxKind.MethodDeclaration))
                {
                    var method = (MethodDeclarationSyntax) ancestor;
                    var methodName = method.Identifier.ValueText;

                    if (methodName == "Dispose")
                    {
                        var arity = method.ParameterList.Parameters.Count;
                        context.ReportDiagnostic(Diagnostic.Create(DisposeRule, warningLocation, arity == 0 ? "Dispose()" : "Dispose(bool)", containingType.Name));
                        return;
                    }

                    if (methodName == "GetHashCode")
                    {
                        context.ReportDiagnostic(Diagnostic.Create(GetHashCodeRule, warningLocation, containingType.Name));
                        return;
                    }
                }
                else if (ancestor.IsKind(SyntaxKind.GetAccessorDeclaration))
                {
                    var property = ancestor.Ancestors().OfType<PropertyDeclarationSyntax>(SyntaxKind.PropertyDeclaration).FirstOrDefault();
                    context.ReportDiagnostic(Diagnostic.Create(PropertyGetterRule, warningLocation, property.Identifier.ValueText));
                    return;
                }
                else if (ancestor.IsKind(SyntaxKind.FinallyClause))
                {
                    context.ReportDiagnostic(Diagnostic.Create(FinallyBlockRule, warningLocation));
                    return;
                }
                else if (ancestor.IsKind(SyntaxKind.OperatorDeclaration))
                {
                    var operatorDeclaration = (OperatorDeclarationSyntax) ancestor;
                    if (operatorDeclaration.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) || operatorDeclaration.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                    {
                        var operatorToken = operatorDeclaration.OperatorToken.ValueText;
                        var firstType = operatorDeclaration.ParameterList.Parameters[0].Type.ToString();
                        var secondType = operatorDeclaration.ParameterList.Parameters[1].Type.ToString();
                        context.ReportDiagnostic(Diagnostic.Create(EqualityOperatorRule, warningLocation, operatorToken, firstType, secondType, containingType.Name));
                        return;
                    }
                }
                else if (ancestor.IsKind(SyntaxKind.ConversionOperatorDeclaration))
                {
                    var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax) ancestor;
                    if (conversionOperatorDeclaration.ImplicitOrExplicitKeyword.IsKind(SyntaxKind.ImplicitKeyword))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(ImplicitOperatorRule, warningLocation, conversionOperatorDeclaration.Type.ToString(), containingType.Name));
                        return;
                    }
                }
                else if (ancestor.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    var constructor = (ConstructorDeclarationSyntax) ancestor;
                    if (constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(StaticConstructorRule, warningLocation, containingType.Name));
                        return;
                    }
                }
                else if (ancestor.IsKind(SyntaxKind.DestructorDeclaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(FinalizerRule, warningLocation, containingType.Name));
                    return;
                }
            }
        }
    }
}