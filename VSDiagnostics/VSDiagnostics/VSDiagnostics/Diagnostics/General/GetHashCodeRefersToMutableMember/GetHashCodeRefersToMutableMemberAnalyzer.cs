using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.General.GetHashCodeRefersToMutableMember
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GetHashCodeRefersToMutableMemberAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.GetHashCodeRefersToMutableFieldAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.GetHashCodeRefersToMutableFieldAnalyzerTitle;

        internal static DiagnosticDescriptor Rule =>
            new DiagnosticDescriptor(DiagnosticId.GetHashCodeRefersToMutableField, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        
        public override void Initialize(AnalysisContext context) =>
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol)context.Symbol;
            var semanticModel = context.Compilation.GetSemanticModel(context.Symbol.Locations[0].SourceTree);

            var getHashCode = GetHashCodeSymbol(namedType);
            if (getHashCode == null) { return; }

            var getHashCodeLocation = getHashCode.Locations[0];
            var root = getHashCodeLocation?.SourceTree.GetRoot(context.CancellationToken);
            if (root == null)
            {
                return;
            }

            var getHashCodeNode = (MethodDeclarationSyntax)root.FindNode(getHashCodeLocation.SourceSpan);
            var nodes = getHashCodeNode.DescendantNodes(descendIntoChildren: target => true);

            var identifierNameNodes = nodes.OfType<IdentifierNameSyntax>(SyntaxKind.IdentifierName);
            foreach (var node in identifierNameNodes)
            {
                var symbol = semanticModel.GetSymbolInfo(node).Symbol;
                if (symbol == null)
                {
                    continue;
                }

                if (symbol.Kind == SymbolKind.Field)
                {
                    var fieldIsMutableOrStatic = FieldIsMutableOrStatic((IFieldSymbol) symbol);
                    if (fieldIsMutableOrStatic.Item1)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, getHashCode.Locations[0],
                            fieldIsMutableOrStatic.Item2));
                    }
                }
                else if (symbol.Kind == SymbolKind.Property)
                {
                    var propertyIsMutable = PropertyIsMutable((IPropertySymbol) symbol, root);
                    if (propertyIsMutable.Item1)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, getHashCode.Locations[0],
                            propertyIsMutable.Item2));
                    }
                }
            }
        }

        private IMethodSymbol GetHashCodeSymbol(INamedTypeSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (!(member is IMethodSymbol))
                {
                    continue;
                }

                var method = (IMethodSymbol)member;
                if (method.MetadataName == nameof(GetHashCode) && method.Parameters.Length == 0)
                {
                    return method;
                }
            }

            return null;
        }
        
        private Tuple<bool, string> FieldIsMutableOrStatic(IFieldSymbol field)
        {
            var description = string.Empty;
            var returnResult = false;

            if (field.IsConst)
            {
                description += "const ";
                returnResult = true;
            }

            // constant fields are marked static
            if (field.IsStatic && !field.IsConst)
            {
                description += "static ";
                returnResult = true;
            }
            
            // constant fields are marked non-readonly
            if (!field.IsReadOnly && !field.IsConst)
            {
                description += "non-readonly ";
                returnResult = true;
            }

            if (!field.Type.IsValueType && field.Type.SpecialType != SpecialType.System_String)
            {
                description += "non-value type, non-string ";
                returnResult = true;
            }

            return Tuple.Create(returnResult, description + "field " + field.Name);
        }

        private Tuple<bool, string> PropertyIsMutable(IPropertySymbol property, SyntaxNode root)
        {
            var description = string.Empty;
            var returnResult = false;

            if (property.IsStatic)
            {
                description += "static ";
                returnResult = true;
            }

            if (!property.Type.IsValueType && property.Type.SpecialType != SpecialType.System_String)
            {
                description += "non-value type, non-string ";
                returnResult = true;
            }

            if (property.SetMethod != null)
            {
                description += "settable ";
                returnResult = true;
            }

            var propertyLocation = property.Locations[0];
            var propertyNode = (PropertyDeclarationSyntax) root.FindNode(propertyLocation.SourceSpan);

            // ensure getter does not have body
            // the property has to have at least one of {get, set}, and it doesn't have a set (see above)
            // this will not have an NRE in First()
            // the accessor list might be null if it uses the arrow operator `=>`
            if (propertyNode.AccessorList == null ||
                propertyNode.AccessorList.Accessors[0].Body != null)
            {
                description += "property with bodied getter ";
                returnResult = true;
            }
            else
            {
                description += "property ";
            }

            return Tuple.Create(returnResult, description + property.Name);
        }
    }
}