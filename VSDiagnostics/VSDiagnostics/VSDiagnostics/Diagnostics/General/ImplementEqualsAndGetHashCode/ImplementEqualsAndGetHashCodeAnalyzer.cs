using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.ImplementEqualsAndGetHashCode
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImplementEqualsAndGetHashCodeAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Hidden;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.ImplementEqualsAndGetHashCodeAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.ImplementEqualsAndGetHashCodeAnalyzerTitle;

        internal static DiagnosticDescriptor Rule =>
            new DiagnosticDescriptor(DiagnosticId.ImplementEqualsAndGetHashCode, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

        private void AnalyzeSymbol(SymbolAnalysisContext symbol)
        {
            IMethodSymbol objectEquals;
            IMethodSymbol objectGetHashCode;

            var namedType = (INamedTypeSymbol)symbol.Symbol;
            GetEqualsAndGetHashCodeSymbols(namedType, out objectEquals, out objectGetHashCode);

            if (objectEquals != null || objectGetHashCode != null) { return; }

            if (MembersContainNonStaticFieldOrProperty(namedType.GetMembers()))
            {
                // it is only reported at the main location--the other locations are (apparently) just passed as data
                // todo--ask the Roslyn team about this
                for (var i = 0; i < namedType.Locations.Count(); i++)
                {
                    symbol.ReportDiagnostic(Diagnostic.Create(Rule, namedType.Locations[i],
                    namedType.Locations.RemoveAt(i),
                    namedType.TypeKind == TypeKind.Class ? "Class" : "Struct", namedType.Name));
                }
            }
        }

        private void GetEqualsAndGetHashCodeSymbols(INamedTypeSymbol symbol, out IMethodSymbol equalsSymbol, out IMethodSymbol getHashCodeSymbol)
        {
            equalsSymbol = null;
            getHashCodeSymbol = null;

            foreach (var member in symbol.GetMembers())
            {
                if (!(member is IMethodSymbol))
                {
                    continue;
                }

                var method = (IMethodSymbol)member;
                if (method.MetadataName == nameof(Equals) && method.Parameters.Count() == 1)
                {
                    equalsSymbol = method;
                }

                if (method.MetadataName == nameof(GetHashCode) && !method.Parameters.Any())
                {
                    getHashCodeSymbol = method;
                }
            }
        }

        private bool MembersContainNonStaticFieldOrProperty(ImmutableArray<ISymbol> members)
        {
            foreach (var member in members)
            {
                if (member.Kind != SymbolKind.Field && member.Kind != SymbolKind.Property)
                {
                    continue;
                }

                if (member.IsStatic)
                {
                    continue;
                }

                if (member.Kind == SymbolKind.Field)
                {
                    return true;
                }

                var property = (IPropertySymbol) member;
                if (property.GetMethod != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}