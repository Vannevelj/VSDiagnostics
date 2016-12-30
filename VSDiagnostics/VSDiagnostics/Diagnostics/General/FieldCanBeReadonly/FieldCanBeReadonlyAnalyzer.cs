using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.FieldCanBeReadonly
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldCanBeReadonlyAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.FieldCanBeReadonlyAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.FieldCanBeReadonlyAnalyzerTitle;

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId.FieldCanBeReadonly, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ClassDeclaration);

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var classSymbol = (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node);
            if (classSymbol.TypeKind != TypeKind.Class &&
                classSymbol.TypeKind != TypeKind.Struct)
            {
                return;
            }

            var nonReadonlyFieldMembers = new HashSet<IFieldSymbol>();
            foreach (var item in classSymbol.GetMembers())
            {
                var symbol = item as IFieldSymbol;
                if (symbol != null && symbol.DeclaredAccessibility == Accessibility.Private && !symbol.IsReadOnly)
                {
                    nonReadonlyFieldMembers.Add(symbol);
                }
            }

            var membersCanBeReadonly = nonReadonlyFieldMembers;
            foreach (var syntaxReference in classSymbol.DeclaringSyntaxReferences)
            {
                var classNode = syntaxReference.SyntaxTree.GetRoot().FindNode(syntaxReference.Span);
                membersCanBeReadonly = WalkTree(context.SemanticModel, classNode, membersCanBeReadonly);
            }

            foreach (var symbol in membersCanBeReadonly)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name));
            }
        }

        private static HashSet<IFieldSymbol> WalkTree(SemanticModel model, SyntaxNode node, HashSet<IFieldSymbol> unassignedSymbols)
        {
            foreach (var child in node.ChildNodes())
            {
                var symbol = model.GetSymbolInfo(child).Symbol as IFieldSymbol;
                if (symbol != null && unassignedSymbols.Contains(symbol))
                {
                    ConstructorDeclarationSyntax ctorNode;
                    if (IsDescendentOfCtor(child, out ctorNode))
                    {
                        if (!ctorNode.Modifiers.ContainsAny(SyntaxKind.StaticKeyword) && symbol.IsStatic)
                        {
                            unassignedSymbols.Remove(symbol);
                        }

                        // assignments in the ctor don't matter other than the modifiers point checked above
                        continue;
                    }

                    var assignmentNode = child.Parent as AssignmentExpressionSyntax;
                    if (assignmentNode?.Left == child)
                    {
                        unassignedSymbols.Remove(symbol);
                    }

                    var argumentNode = child.Parent as ArgumentSyntax;
                    if (argumentNode?.RefOrOutKeyword != null)
                    {
                        unassignedSymbols.Remove(symbol);
                    }

                    var postFixExpressionNode = child.Parent as PostfixUnaryExpressionSyntax;
                    if (postFixExpressionNode != null)
                    {
                        unassignedSymbols.Remove(symbol);
                    }

                    var preFixExpressionNode = child.Parent as PrefixUnaryExpressionSyntax;
                    if (preFixExpressionNode != null)
                    {
                        unassignedSymbols.Remove(symbol);
                    }
                }

                unassignedSymbols = WalkTree(model, child, unassignedSymbols);
            }

            return unassignedSymbols;
        }

        private static bool IsDescendentOfCtor(SyntaxNode node, out ConstructorDeclarationSyntax ctor)
        {
            ctor = null;

            var parent = node.Parent;
            while (!parent.IsKind(SyntaxKind.ClassDeclaration) && !parent.IsKind(SyntaxKind.StructDeclaration))
            {
                if (parent.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    ctor = (ConstructorDeclarationSyntax)parent;
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }
    }
}