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

        public static SyntaxToken WithConvention(this SyntaxToken identifier, NamingConvention namingConvention)
        {
            // int @class = 5;
            if (identifier.IsVerbatimIdentifier())
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
                case NamingConvention.InterfacePrefixUpperCamelCase:
                    newValue = GetInterfacePrefixUpperCamelCaseIdentifier(originalValue);
                    break;
                default:
                    throw new ArgumentException(nameof(namingConvention));
            }

            return SyntaxFactory.Identifier(identifier.LeadingTrivia, newValue, identifier.TrailingTrivia);
        }

        // lowerCamelCase
        private static string GetLowerCamelCaseIdentifier(string identifier)
        {
            if (ContainsSpecialCharacters(identifier))
            {
                return identifier;
            }

            var normalizedString = GetNormalizedString(identifier);

            if (normalizedString.Length >= 1)
            {
                return char.ToLower(normalizedString[0]) + normalizedString.Substring(1);
            }
            return identifier;
        }

        // UpperCamelCase
        private static string GetUpperCamelCaseIdentifier(string identifier)
        {
            if (ContainsSpecialCharacters(identifier))
            {
                return identifier;
            }

            var normalizedString = GetNormalizedString(identifier);

            if (normalizedString.Length == 0)
            {
                return identifier;
            }
            return char.ToUpper(normalizedString[0]) + normalizedString.Substring(1);
        }

        // _lowerCamelCase
        private static string GetUnderscoreLowerCamelCaseIdentifier(string identifier)
        {
            if (ContainsSpecialCharacters(identifier, '_'))
            {
                return identifier;
            }

            var normalizedString = GetNormalizedString(identifier);
            if (normalizedString.Length == 0)
            {
                return identifier;
            }

            if (normalizedString.Length == 1)
            {
                return "_" + char.ToLower(normalizedString[0]);
            }

            // _Var
            if (normalizedString[0] == '_' && char.IsUpper(normalizedString[1]))
            {
                return "_" + char.ToLower(normalizedString[1]) + normalizedString.Substring(2);
            }

            // Var
            if (char.IsUpper(normalizedString[0]))
            {
                return "_" + char.ToLower(normalizedString[0]) + normalizedString.Substring(1);
            }

            // var
            if (char.IsLower(normalizedString[0]))
            {
                return "_" + normalizedString;
            }

            return normalizedString;
        }

        // IInterface
        private static string GetInterfacePrefixUpperCamelCaseIdentifier(string identifier)
        {
            if (ContainsSpecialCharacters(identifier))
            {
                return identifier;
            }

            var normalizedString = GetNormalizedString(identifier);

            if (normalizedString.Length <= 1)
            {
                return identifier;
            }

            // iSomething
            if (normalizedString[0] == 'i' && char.IsUpper(normalizedString[1]))
            {
                return "I" + normalizedString.Substring(1);
            }

            // isomething
            if (char.IsLower(normalizedString[0]) && char.IsLower(normalizedString[1]))
            {
                return "I" + char.ToUpper(normalizedString[0]) + normalizedString.Substring(1);
            }

            // Isomething
            if (normalizedString[0] == 'I' && char.IsLower(normalizedString[1]))
            {
                return "I" + char.ToUpper(normalizedString[1]) + normalizedString.Substring(2);
            }

            return normalizedString;
        }

        private static string GetNormalizedString(string input)
        {
            return new string(input.ToCharArray().Where(x => char.IsLetter(x) || char.IsNumber(x)).ToArray());
        }

        private static bool ContainsSpecialCharacters(string input, params char[] allowedCharacters)
        {
            return !input.ToCharArray().All(x => char.IsLetter(x) || char.IsNumber(x) || allowedCharacters.Contains(x));
        }
    }
}