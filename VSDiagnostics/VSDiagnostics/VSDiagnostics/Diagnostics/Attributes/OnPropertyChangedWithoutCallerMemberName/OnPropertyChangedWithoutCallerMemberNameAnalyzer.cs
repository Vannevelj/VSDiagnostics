using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
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
        private static readonly string Message = VSDiagnosticsResources.ObsoleteAttributeWithoutReasonAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ObsoleteAttributeWithoutReasonAnalyzerTitle;

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

            context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.GetLocation()));
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