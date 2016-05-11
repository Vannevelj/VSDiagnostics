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

namespace VSDiagnostics.Diagnostics.Tests.TestMethodWithoutTestAttribute
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestMethodWithoutTestAttributeAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.TestsCategory;
        private static readonly string Message = VSDiagnosticsResources.TestMethodWithoutTestAttributeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.TestMethodWithoutTestAttributeAnalyzerTitle;
        private static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.TestMethodWithoutTestAttribute, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax) context.Node;
            
            // Check if we're in a unit-test context
            // For NUnit and MSTest we can see if the enclosing class/struct has a [TestClass] or [TestFixture] attribute
            // For xUnit.NET we will have to see if there are other methods in the current class that contain a [Fact] attribute

            var enclosingType = method.GetEnclosingTypeNode();
            if (!enclosingType.IsKind(SyntaxKind.ClassDeclaration) && !enclosingType.IsKind(SyntaxKind.StructDeclaration))
            {
                return;
            }

            var symbol = context.SemanticModel.GetDeclaredSymbol(enclosingType) as INamedTypeSymbol;
            if (symbol == null)
            {
                return;
            }

            var isTestClass = false;
            foreach (var attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass.Name == "TestClass" || attribute.AttributeClass.Name == "TestFixture")
                {
                    isTestClass = true;
                    break;
                }
            }

            // If it has different attributes then we won't bother with it either
            if (method.AttributeLists.SelectMany(x => x.Attributes).Any())
            {
                return;
            }

            if (!isTestClass)
            {
                // Look at other methods in the class to see if they have a test attribute
                // We do this only for xUnit.NET because the others should already have been caught with the previous test
                // If they weren't, it means the entire class wasn't marked as a test which is not in the scope of this analyzer

                foreach (var member in enclosingType.DescendantNodes().OfType<MethodDeclarationSyntax>(SyntaxKind.MethodDeclaration))
                {
                    foreach (var attributeList in member.AttributeLists)
                    {
                        foreach (var attribute in attributeList.Attributes)
                        {
                            if (attribute.Name.ToString() == "Fact" || attribute.Name.ToString() == "Theory")
                            {
                                isTestClass = true;
                            }
                        }
                    }
                }
            }

            if (!isTestClass)
            {
                return;
            }

            var returnType = context.SemanticModel.GetTypeInfo(method.ReturnType).Type;
            var voidType = context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Void);
            var taskType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
            var taskTType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            if (!(returnType.Equals(voidType) || returnType.Equals(taskType) || returnType.OriginalDefinition.Equals(taskTType)))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier));
        }
    }
}
