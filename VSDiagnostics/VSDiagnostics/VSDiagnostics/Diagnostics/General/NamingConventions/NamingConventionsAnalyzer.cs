using System.Collections.Immutable;
using System.Linq;
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
                SyntaxKind.Parameter);
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
                    SyntaxToken conventionedIdentifier;

                    if (nodeAsField.Modifiers.Any(x => new[] { "internal", "protected", "public" }.Contains(x.Text)))
                    {
                        conventionedIdentifier = variable.Identifier.WithConvention(NamingConvention.UpperCamelCase);
                    }
                    else if (nodeAsField.Modifiers.Any(x => x.Text == "private"))
                    {
                        conventionedIdentifier = variable.Identifier.WithConvention(NamingConvention.UnderscoreLowerCamelCase);
                    }
                    else
                    {
                        return; // Code is in incomplete state
                    }

                    if (conventionedIdentifier.Text != variable.Identifier.Text)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, variable.Identifier.GetLocation(), "field", variable.Identifier.Text, conventionedIdentifier.Text));
                    }
                }

                return;
            }

            var nodeAsProperty = context.Node as PropertyDeclarationSyntax;
            if (nodeAsProperty != null)
            {
                var conventionedIdentifier = nodeAsProperty.Identifier.WithConvention(NamingConvention.UpperCamelCase);
                if (conventionedIdentifier.Text != nodeAsProperty.Identifier.Text)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, nodeAsProperty.Identifier.GetLocation(), "property", nodeAsProperty.Identifier.Text, conventionedIdentifier.Text));
                }
                return;
            }

            var nodeAsMethod = context.Node as MethodDeclarationSyntax;
            if (nodeAsMethod != null)
            {
                var conventionedIdentifier = nodeAsMethod.Identifier.WithConvention(NamingConvention.UpperCamelCase);
                if (conventionedIdentifier.Text != nodeAsMethod.Identifier.Text)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, nodeAsMethod.Identifier.GetLocation(), "method", nodeAsMethod.Identifier.Text, conventionedIdentifier.Text));
                }
                return;
            }

            var nodeAsClass = context.Node as ClassDeclarationSyntax;
            if (nodeAsClass != null)
            {
                var conventionedIdentifier = nodeAsClass.Identifier.WithConvention(NamingConvention.UpperCamelCase);
                if (conventionedIdentifier.Text != nodeAsClass.Identifier.Text)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, nodeAsClass.Identifier.GetLocation(), "class", nodeAsClass.Identifier.Text, conventionedIdentifier.Text));
                }
                return;
            }

            var nodeAsInterface = context.Node as InterfaceDeclarationSyntax;
            if (nodeAsInterface != null)
            {
                var conventionedIdentifier = nodeAsInterface.Identifier.WithConvention(NamingConvention.InterfacePrefixUpperCamelCase);
                if (conventionedIdentifier.Text != nodeAsInterface.Identifier.Text)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, nodeAsInterface.Identifier.GetLocation(), "interface", nodeAsInterface.Identifier.Text, conventionedIdentifier.Text));
                }
                return;
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
                    var conventionedIdentifier = variable.Identifier.WithConvention(NamingConvention.LowerCamelCase);
                    if (conventionedIdentifier.Text != variable.Identifier.Text)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, variable.Identifier.GetLocation(), "local", variable.Identifier.Text, conventionedIdentifier.Text));
                    }
                }

                return;
            }

            var nodeAsParameter = context.Node as ParameterSyntax;
            if (nodeAsParameter != null)
            {
                var conventionedIdentifier = nodeAsParameter.Identifier.WithConvention(NamingConvention.InterfacePrefixUpperCamelCase);
                if (conventionedIdentifier.Text != nodeAsParameter.Identifier.Text)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, nodeAsParameter.Identifier.GetLocation(), "parameter", nodeAsParameter.Identifier.Text, conventionedIdentifier.Text));
                }
            }
        }
    }
}