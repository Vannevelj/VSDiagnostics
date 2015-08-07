using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.SingleEmptyConstructor
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SingleEmptyConstructorAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(SingleEmptyConstructorAnalyzer);
        private const string Message = "Type \"{0}\" has a redundant default constructor.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Your constructor is the same as a default constructor and can be removed.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = context.Node as ConstructorDeclarationSyntax;
            if (constructorDeclaration == null)
            {
                return;
            }

            // ctor must be public
            if (!constructorDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return;
            }

            // ctor must not have parameters
            if (constructorDeclaration.ParameterList.Parameters.Any())
            {
                return;
            }

            // ctor must have no body statements
            if (constructorDeclaration.Body.Statements.Any())
            {
                return;
            }

            // ctor must not contain comments
            if (constructorDeclaration.Body.CloseBraceToken.LeadingTrivia.Any(t => t.IsCommentTrivia()))
            {
                return;
            }

            // ctor must not have attributes
            if (constructorDeclaration.AttributeLists.Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, constructorDeclaration.GetLocation(), constructorDeclaration.Identifier));
        }
    }
}