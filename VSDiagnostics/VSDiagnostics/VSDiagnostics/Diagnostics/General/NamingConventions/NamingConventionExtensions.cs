using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NamingConventions
{
    internal static class NamingConventionExtensions
    {
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
        internal static string GetLowerCamelCaseIdentifier(string identifier)
        {
            if (ContainsSpecialCharacters(identifier, '_'))
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
        internal static string GetUpperCamelCaseIdentifier(string identifier)
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
            return char.ToUpper(normalizedString[0]) + normalizedString.Substring(1);
        }

        // _lowerCamelCase
        internal static string GetUnderscoreLowerCamelCaseIdentifier(string identifier)
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
        internal static string GetInterfacePrefixUpperCamelCaseIdentifier(string identifier)
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

            // iSomething
            if (normalizedString.Length >= 2 && normalizedString[0] == 'i' && char.IsUpper(normalizedString[1]))
            {
                return "I" + normalizedString.Substring(1);
            }

            // Something, something, isomething
            if (normalizedString[0] != 'I')
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
            var sb = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) || char.IsNumber(input[i]))
                {
                    sb.Append(input[i]);
                }

                if (input[i] == '_' && i + 1 < input.Length && input[i + 1] != '_')
                {
                    sb.Append(char.ToUpper(input[++i]));
                }
            }
            return sb.ToString();
        }

        private static bool ContainsSpecialCharacters(string input, params char[] allowedCharacters)
        {
            return !input.ToCharArray().All(x => char.IsLetter(x) || char.IsNumber(x) || allowedCharacters.Contains(x));
        }
    }
}