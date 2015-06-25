using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace VSDiagnostics.Utilities
{
    public static class Extensions
    {
        public static bool InheritsFrom(this ISymbol typeSymbol, Type type)
        {
            if (typeSymbol == null || type == null)
            {
                return false;
            }

            var baseType = typeSymbol;
            while (baseType != null)
            {
                if (baseType.MetadataName == type.Name)
                {
                    return true;
                }
                baseType = ((ITypeSymbol) typeSymbol).BaseType;
            }

            return false;
        }

        public static bool IsCommentTrivia(this SyntaxTrivia trivia)
        {
            var commentTrivias = new[]
            {
                SyntaxKind.SingleLineCommentTrivia,
                SyntaxKind.MultiLineCommentTrivia,
                SyntaxKind.DocumentationCommentExteriorTrivia,
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia,
                SyntaxKind.EndOfDocumentationCommentToken,
                SyntaxKind.XmlComment,
                SyntaxKind.XmlCommentEndToken,
                SyntaxKind.XmlCommentStartToken
            };

            return commentTrivias.Any(x => trivia.IsKind(x));
        }

        public static bool IsWhitespaceTrivia(this SyntaxTrivia trivia)
        {
            var whitespaceTrivia = new[]
            {
                SyntaxKind.WhitespaceTrivia,
                SyntaxKind.EndOfLineTrivia
            };

            return whitespaceTrivia.Any(x => trivia.IsKind(x));
        }
    }
}