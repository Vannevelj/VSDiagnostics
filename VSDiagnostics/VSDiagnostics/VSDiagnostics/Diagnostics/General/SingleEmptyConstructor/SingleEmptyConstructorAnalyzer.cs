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
    class SingleEmptyConstructorAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "General";
        private const string DiagnosticId = nameof(SingleEmptyConstructorAnalyzer);
        private const string Message = "Type \"{0}\" has a redundant default constructor.";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;
        private const string Title = "Your constructor is the same as a default constructor and can be removed.";

        internal static DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize (AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeSymbol (SyntaxNodeAnalysisContext context)
        {
            var ctorExpression = context.Node as ConstructorDeclarationSyntax;
            if (ctorExpression == null)
            {
                return;
            }
            
            // ctor must be public
            if (!ctorExpression.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return;
            }

            // ctor must not have parameters
            if (ctorExpression.ParameterList.Parameters.Any())
            {
                return;
            }

            // ctor must have no body statements
            if (ctorExpression.Body.Statements.Any())
            {
                return;
            }

            // ctor must not contain comments
            if (ctorExpression.Body.CloseBraceToken.LeadingTrivia.Any(t => t.IsCommentTrivia()))
            {
                return;
            }

            // ctor must not have attributes
            if (ctorExpression.AttributeLists.Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, ctorExpression.GetLocation(), ctorExpression.Identifier));
        }
    }
}
