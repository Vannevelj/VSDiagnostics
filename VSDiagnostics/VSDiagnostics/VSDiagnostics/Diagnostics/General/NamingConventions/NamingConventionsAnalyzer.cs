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
        public const string DiagnosticId = nameof(NamingConventionsAnalyzer);
        internal const string Title = "A member does not follow naming conventions.";
        internal const string Message = "The {0} {1} does not follow naming conventions. Should be {2}.";
        internal const string Category = "General";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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
                SyntaxKind.StructDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var nodeAsField = context.Node as FieldDeclarationSyntax;
            if (nodeAsField != null)
            {
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
                        CheckNaming(variable.Identifier, "field", NamingConvention.UnderscoreLowerCamelCase, context);
                    }
                    else
                    {
                        return; // Code is in an incomplete state
                    }
                }

                return;
            }

            var nodeAsProperty = context.Node as PropertyDeclarationSyntax;
            if (nodeAsProperty != null)
            {
                CheckNaming(nodeAsProperty.Identifier, "property", NamingConvention.UpperCamelCase, context);
            }

            var nodeAsMethod = context.Node as MethodDeclarationSyntax;
            if (nodeAsMethod != null)
            {
                CheckNaming(nodeAsMethod.Identifier, "method", NamingConvention.UpperCamelCase, context);
            }

            var nodeAsClass = context.Node as ClassDeclarationSyntax;
            if (nodeAsClass != null)
            {
                CheckNaming(nodeAsClass.Identifier, "class", NamingConvention.UpperCamelCase, context);
            }

            var nodeAsInterface = context.Node as InterfaceDeclarationSyntax;
            if (nodeAsInterface != null)
            {
                CheckNaming(nodeAsInterface.Identifier, "interface", NamingConvention.InterfacePrefixUpperCamelCase, context);
            }

            var nodeAsLocal = context.Node as LocalDeclarationStatementSyntax;
            if (nodeAsLocal != null)
            {
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

            var nodeAsParameter = context.Node as ParameterSyntax;
            if (nodeAsParameter != null)
            {
                CheckNaming(nodeAsParameter.Identifier, "parameter", NamingConvention.LowerCamelCase, context);
                return;
            }

            var nodeAsStruct = context.Node as StructDeclarationSyntax;
            if (nodeAsStruct != null)
            {
                CheckNaming(nodeAsStruct.Identifier, "struct", NamingConvention.UpperCamelCase, context);
            }
        }

        private void CheckNaming(SyntaxToken currentIdentifier, string memberType, NamingConvention convention, SyntaxNodeAnalysisContext context)
        {
            var conventionedIdentifier = currentIdentifier.WithConvention(convention);
            if (conventionedIdentifier.Text != currentIdentifier.Text)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, currentIdentifier.GetLocation(), memberType, currentIdentifier.Text, conventionedIdentifier.Text));
            }
        }
    }
}