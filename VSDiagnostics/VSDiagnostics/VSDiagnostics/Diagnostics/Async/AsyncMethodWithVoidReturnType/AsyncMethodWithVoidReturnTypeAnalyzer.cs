using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Async.AsyncMethodWithVoidReturnType
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodWithVoidReturnTypeAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.AsyncCategory;
        private static readonly string Message = VSDiagnosticsResources.AsyncMethodWithVoidReturnTypeMessage;
        private static readonly string Title = VSDiagnosticsResources.AsyncMethodWithVoidReturnTypeTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.AsyncMethodWithVoidReturnType, Title, Message, Category, Severity, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.MethodDeclaration);

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax) context.Node;

            // Method has to return void
            var returnType = context.SemanticModel.GetTypeInfo(method.ReturnType);
            if (returnType.Type == null || returnType.Type.SpecialType != SpecialType.System_Void)
            {
                return;
            }
            
            var isAsync = false;
            foreach (var modifier in method.Modifiers)
            {
                // Method has to be marked async
                if (modifier.IsKind(SyntaxKind.AsyncKeyword))
                {
                    isAsync = true;
                }

                // Partial methods can only have a void return type
                if (modifier.IsKind(SyntaxKind.PartialKeyword))
                {
                    return;
                }
            }

            if (!isAsync)
            {
                return;
            }

            // Event handlers can only have a void return type
            if (method.ParameterList?.Parameters.Count == 2)
            {
                var parameters = method.ParameterList.Parameters;
                var firstArgumentType = context.SemanticModel.GetTypeInfo(parameters[0].Type);
                var isFirstArgumentObject = firstArgumentType.Type != null &&
                                            firstArgumentType.Type.SpecialType == SpecialType.System_Object;


                var secondArgumentType = context.SemanticModel.GetTypeInfo(parameters[1].Type);
                var isSecondArgumentEventArgs = secondArgumentType.Type != null &&
                                                secondArgumentType.Type.InheritsFrom(typeof(EventArgs));

                if (isFirstArgumentObject && isSecondArgumentEventArgs)
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, method.GetLocation(), method.Identifier.ValueText));
        }
    }
}
