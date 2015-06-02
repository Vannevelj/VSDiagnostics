using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public static SyntaxToken WithConvention(this SyntaxToken identifier, NamingConvention namingConvention)
        {
            // int @class = 5;
            if(identifier.IsVerbatimIdentifier())
            {
                return identifier;
            }

            // int cl\u0061ss = 5;
            if (identifier.Text.Contains("\\"))
            {
                return identifier;
            }

            var originalValue = identifier.ValueText;
            string newValue;

            switch (namingConvention)
            {
                case NamingConvention.LowerCamelCase:
                    newValue = GetLowerCamelCaseIdentifier(originalValue);
                    break;
                case NamingConvention.UpperCamelCase:
                    newValue = GetUpperCamelCaseIdentifier(originalValue);
                    break;
                case NamingConvention.UnderscoreLowerCamelCase:
                    newValue = GetUnderscoreLowerCamelCaseIdentifier(originalValue);
                    break;
                default:
                    throw new ArgumentException(nameof(namingConvention));
            }

            return SyntaxFactory.Identifier(identifier.LeadingTrivia, newValue, identifier.TrailingTrivia);
        }

        private static string GetLowerCamelCaseIdentifier(string identifier)
        {
            return identifier[0].ToString().ToLower() + identifier.Substring(1);  
        }

        private static string GetUpperCamelCaseIdentifier(string identifier)
        {
            return identifier[0].ToString().ToUpper() + identifier.Substring(1);
        }

        private static string GetUnderscoreLowerCamelCaseIdentifier(string identifier)
        {
            return "_" + identifier[0].ToString().ToLower() + identifier.Substring(1);
        }
    }
}