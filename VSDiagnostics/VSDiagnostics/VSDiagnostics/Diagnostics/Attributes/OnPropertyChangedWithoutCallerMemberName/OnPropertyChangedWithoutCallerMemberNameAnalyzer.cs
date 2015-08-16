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
        private const string DiagnosticId = nameof(OnPropertyChangedWithoutCallerMemberNameAnalyzer);
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.AttributesCategory;
        private static readonly string Message = VSDiagnosticsResources.OnPropertyChangedWithoutCallerMemberNameAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.OnPropertyChangedWithoutCallerMemberNameAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax) context.Node;

            // class must implement INotifyPropertyChanged
            if (!classDeclaration.ImplementsInterface(context.SemanticModel, typeof(INotifyPropertyChanged)))
            {
                return;
            }

            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                // method name must be "OnPropertyChanged"
                var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(method);
                if (declaredSymbol == null || declaredSymbol.MetadataName != "OnPropertyChanged")
                {
                    continue;
                }

                // method must have just one parameter
                if (method.ParameterList.Parameters.Count != 1)
                {
                    continue;
                }

                var param = method.ParameterList.Parameters.First();
                var paramType = param.Type as PredefinedTypeSyntax;

                // and that parameter must be of type string
                if (paramType == null || !paramType.Keyword.IsKind(SyntaxKind.StringKeyword))
                {
                    continue;
                }

                // parameter must not have CallerMemberNameAttribute
                if (param.AttributeLists.Any(a => a.Attributes.Any() && a.Attributes.Any(t =>
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(t).Symbol;
                    return symbol != null && symbol.ContainingSymbol.MetadataName == typeof (CallerMemberNameAttribute).Name;
                })))
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(Rule, method.GetLocation()));
                return;
            }
        }
    }
}