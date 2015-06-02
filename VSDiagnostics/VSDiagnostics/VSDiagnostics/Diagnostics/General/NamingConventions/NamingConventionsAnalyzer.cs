using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

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
            throw new NotImplementedException();
        }
    }
}