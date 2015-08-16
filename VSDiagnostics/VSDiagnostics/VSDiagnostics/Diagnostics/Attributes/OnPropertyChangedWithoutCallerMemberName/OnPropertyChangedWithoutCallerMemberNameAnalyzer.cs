using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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
            if (!ClassImplementsINotifyPropertyChanged(context.SemanticModel, classDeclaration))
            {
                return;
            }

            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(method);
                if (declaredSymbol == null || declaredSymbol.MetadataName != "OnPropertyChanged")
                {
                    continue;
                }

                if (method.ParameterList.Parameters.Count != 1)
                {
                    continue;
                }

                var param = method.ParameterList.Parameters.First();
                var paramType = method.ParameterList.Parameters.First().Type as PredefinedTypeSyntax;
                var value = param.Default.Value as LiteralExpressionSyntax;

                if (paramType == null || value == null || !paramType.Keyword.IsKind(SyntaxKind.StringKeyword))
                {
                    continue;
                }

                if (value.Token.ValueText == "" &&
                    param.AttributeLists.Any(a => a.Attributes.Any() && a.Attributes.Any(t =>
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

        private bool ClassImplementsINotifyPropertyChanged(SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
        {
            var declaredSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            return declaredSymbol != null &&
                   (declaredSymbol.Interfaces.Any(i => i.MetadataName == typeof (INotifyPropertyChanged).Name) ||
                    declaredSymbol.BaseType.MetadataName == typeof (INotifyPropertyChanged).Name);

            // For some peculiar reason, "class Foo : INotifyPropertyChanged" doesn't have any interfaces,
            // But "class Foo : IFoo, INotifyPropertyChanged" has two.
            // However, the BaseType for the first is the "INotifyPropertyChanged" symbol.
            // Also, "class Foo : INotifyPropertyChanged, IFoo" has just one - "IFoo",
            // But the BaseType again is "INotifyPropertyChanged".
        }
    }
}