using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NamingConventions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamingConventionsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private static readonly string Category = VSDiagnosticsResources.GeneralCategory;
        private static readonly string Message = VSDiagnosticsResources.NamingConventionsAnalyzerMessage;
        private static readonly string Title = VSDiagnosticsResources.NamingConventionsAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.NamingConventions, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.LocalDeclarationStatement,
                SyntaxKind.Parameter,
                SyntaxKind.StructDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.EnumMemberDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.IsKind(SyntaxKind.FieldDeclaration))
            {
                var nodeAsField = (FieldDeclarationSyntax)context.Node;

                if (nodeAsField.Declaration == null)
                {
                    return;
                }

                foreach (var variable in nodeAsField.Declaration.Variables)
                {
                    var modifiers = nodeAsField.Modifiers;

                    if (modifiers.Any(SyntaxKind.InternalKeyword) ||
                        modifiers.Any(SyntaxKind.ProtectedKeyword) ||
                        modifiers.Any(SyntaxKind.PublicKeyword))
                    {
                        CheckNaming(variable.Identifier, "field", NamingConvention.UpperCamelCase, context);
                    }
                    else if (modifiers.Any(SyntaxKind.PrivateKeyword) ||
                             nodeAsField.Modifiers.Count == 0 /* no access modifier defaults to private */)
                    {
                        if (modifiers.Any(SyntaxKind.StaticKeyword) || modifiers.Any(SyntaxKind.ConstKeyword))
                        {
                            CheckNaming(variable.Identifier, "field", NamingConvention.UpperCamelCase, context);
                        }
                        else
                        {
                            CheckNaming(variable.Identifier, "field", NamingConvention.UnderscoreLowerCamelCase, context);
                        }
                    }
                    else
                    {
                        return; // Code is in an incomplete state
                    }
                }

                return;
            }

            if (context.Node.IsKind(SyntaxKind.PropertyDeclaration))
            {
                var nodeAsProperty = (PropertyDeclarationSyntax)context.Node;
                CheckNaming(nodeAsProperty.Identifier, "property", NamingConvention.UpperCamelCase, context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.MethodDeclaration))
            {
                var nodeAsMethod = (MethodDeclarationSyntax) context.Node;
                CheckNaming(nodeAsMethod.Identifier, "method", NamingConvention.UpperCamelCase, context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.ClassDeclaration))
            {
                var nodeAsClass = (ClassDeclarationSyntax) context.Node;
                CheckNaming(nodeAsClass.Identifier, "class", NamingConvention.UpperCamelCase, context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                var nodeAsInterface = (InterfaceDeclarationSyntax)context.Node;
                CheckNaming(nodeAsInterface.Identifier, "interface", NamingConvention.InterfacePrefixUpperCamelCase,
                    context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                var nodeAsLocal = (LocalDeclarationStatementSyntax) context.Node;
                if (nodeAsLocal.Declaration == null)
                {
                    return;
                }

                foreach (var variable in nodeAsLocal.Declaration.Variables)
                {
                    CheckNaming(variable.Identifier, "local", NamingConvention.LowerCamelCase, context);
                }

                return;
            }
            
            if (context.Node.IsKind(SyntaxKind.Parameter))
            {
                var nodeAsParameter = (ParameterSyntax) context.Node;
                CheckNaming(nodeAsParameter.Identifier, "parameter", NamingConvention.LowerCamelCase, context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.StructDeclaration))
            {
                var nodeAsStruct = (StructDeclarationSyntax) context.Node;
                CheckNaming(nodeAsStruct.Identifier, "struct", NamingConvention.UpperCamelCase, context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.EnumDeclaration))
            {
                var nodeAsEnum = (EnumDeclarationSyntax) context.Node;
                CheckNaming(nodeAsEnum.Identifier, "enum", NamingConvention.UpperCamelCase, context);
                return;
            }

            if (context.Node.IsKind(SyntaxKind.EnumMemberDeclaration))
            {
                var nodeAsEnumMember = (EnumMemberDeclarationSyntax) context.Node;
                CheckNaming(nodeAsEnumMember.Identifier, "enum member", NamingConvention.UpperCamelCase, context);
            }
        }

        private static void CheckNaming(SyntaxToken currentIdentifier, string memberType, NamingConvention convention,
            SyntaxNodeAnalysisContext context)
        {
            var conventionedIdentifier = currentIdentifier.WithConvention(convention);
            if (conventionedIdentifier.Text != currentIdentifier.Text)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(Rule, currentIdentifier.GetLocation(), 
                    ImmutableDictionary.CreateRange( new[] { new KeyValuePair<string, string>("convention", convention.ToString()) }), 
                    memberType, currentIdentifier.Text, conventionedIdentifier.Text));
            }
        }
    }
}