using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Rename;
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

        private static readonly string ConflictingMemberMessage =
            VSDiagnosticsResources.NamingConventionsConflictingMemberAnalyzerMessage;

        private static readonly string ConflictingMemberTitle =
            VSDiagnosticsResources.NamingConventionsConflictingMemberAnalyzerTitle;

        internal static DiagnosticDescriptor Rule
            => new DiagnosticDescriptor(DiagnosticId.NamingConventions, Title, Message, Category, Severity, true);

        internal static DiagnosticDescriptor ConflictingMemberRule
            => new DiagnosticDescriptor(
                DiagnosticId.NamingConventionsConflictingMember,
                ConflictingMemberTitle,
                ConflictingMemberMessage,
                Category,
                Severity,
                true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule, ConflictingMemberRule);

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

            var nodeAsProperty = context.Node as PropertyDeclarationSyntax;
            if (nodeAsProperty != null)
            {
                CheckNaming(nodeAsProperty.Identifier, "property", NamingConvention.UpperCamelCase, context);
                return;
            }

            var nodeAsMethod = context.Node as MethodDeclarationSyntax;
            if (nodeAsMethod != null)
            {
                CheckNaming(nodeAsMethod.Identifier, "method", NamingConvention.UpperCamelCase, context);
                return;
            }

            var nodeAsClass = context.Node as ClassDeclarationSyntax;
            if (nodeAsClass != null)
            {
                CheckNaming(nodeAsClass.Identifier, "class", NamingConvention.UpperCamelCase, context);
                return;
            }

            var nodeAsInterface = context.Node as InterfaceDeclarationSyntax;
            if (nodeAsInterface != null)
            {
                CheckNaming(nodeAsInterface.Identifier, "interface", NamingConvention.InterfacePrefixUpperCamelCase,
                    context);
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
                return;
            }

            var nodeAsEnum = context.Node as EnumDeclarationSyntax;
            if (nodeAsEnum != null)
            {
                CheckNaming(nodeAsEnum.Identifier, "enum", NamingConvention.UpperCamelCase, context);
                return;
            }

            var nodeAsEnumMember = context.Node as EnumMemberDeclarationSyntax;
            if (nodeAsEnumMember != null)
            {
                CheckNaming(nodeAsEnumMember.Identifier, "enum member", NamingConvention.UpperCamelCase, context);
                return;
            }
        }

        private static void CheckNaming(SyntaxToken currentIdentifier, string memberType, NamingConvention convention,
            SyntaxNodeAnalysisContext context)
        {
            var conventionedIdentifier = currentIdentifier.WithConvention(convention);
            if (conventionedIdentifier.Text != currentIdentifier.Text)
            {
                if (!WillConflict(conventionedIdentifier.Text, currentIdentifier.SpanStart, memberType, context,
                    default(SyntaxKind)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, currentIdentifier.GetLocation(), memberType,
                    currentIdentifier.Text, conventionedIdentifier.Text));
                }
            }
        }

        /// <summary>
        /// Verifies there are no existing conflicting members yet
        /// </summary>
        /// <param name="identifierText"></param>
        /// <param name="location"></param>
        /// <param name="memberType"></param>
        /// <param name="context"></param>
        /// <param name="memberKind"></param>
        /// <returns></returns>
        private static bool WillConflict(string identifierText, int location, string memberType, SyntaxNodeAnalysisContext context,
            SyntaxKind memberKind)
        {
            var codefix = new NamingConventionsCodeFix();
            codefix.RenameAsync(context.SemanticModel.SyntaxTree.)


            throw new NotImplementedException();
            
        }
    }
}