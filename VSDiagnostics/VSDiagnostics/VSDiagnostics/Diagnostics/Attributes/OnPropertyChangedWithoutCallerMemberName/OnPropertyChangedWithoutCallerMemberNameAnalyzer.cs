using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.Attributes.OnPropertyChangedWithoutCallerMemberName
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OnPropertyChangedWithoutCallerMemberNameAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.AttributesCategory;

        private static readonly string Message =
            VSDiagnosticsResources.OnPropertyChangedWithoutCallerMemberNameAnalyzerMessage;

        private static readonly string Title =
            VSDiagnosticsResources.OnPropertyChangedWithoutCallerMemberNameAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.OnPropertyChangedWithoutCallerMemberName, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.MethodDeclaration);

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var parentClass = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>(SyntaxKind.ClassDeclaration).FirstOrDefault();
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(parentClass);

            // class must implement INotifyPropertyChanged
            if (!typeSymbol.ImplementsInterfaceOrBaseClass(typeof(INotifyPropertyChanged)))
            {
                return;
            }

            // method name must be "OnPropertyChanged"
            var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
            if (declaredSymbol == null || declaredSymbol.MetadataName != "OnPropertyChanged")
            {
                return;
            }

            // method must have just one parameter
            if (methodDeclaration.ParameterList.Parameters.Count != 1)
            {
                return;
            }

            var param = methodDeclaration.ParameterList.Parameters.First();
            var paramType = param.Type as PredefinedTypeSyntax;

            // and that parameter must be of type string
            if (paramType == null || !paramType.Keyword.IsKind(SyntaxKind.StringKeyword))
            {
                return;
            }

            // parameter must not have CallerMemberNameAttribute
            foreach (var list in param.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
                    if (symbol != null &&
                        symbol.ContainingSymbol.MetadataName == typeof (CallerMemberNameAttribute).Name)
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, methodDeclaration.GetLocation()));
        }
    }
}