using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Diagnostics.General.NamingConventions
{
    internal static class NamingConventionExtensions
    {
        internal static SyntaxToken WithConvention(this SyntaxToken identifier, NamingConvention namingConvention)
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
                    newValue = GetNormalizedString(originalValue, Lower);
                    break;
                case NamingConvention.UpperCamelCase:
                    newValue = GetNormalizedString(originalValue, Upper);
                    break;
                case NamingConvention.UnderscoreLowerCamelCase:
                    newValue = GetNormalizedString(originalValue, UnderscoreLower);
                    break;
                case NamingConvention.InterfacePrefixUpperCamelCase:
                    newValue = GetNormalizedString(originalValue, IUpper);
                    break;
                default:
                    throw new ArgumentException(nameof(namingConvention));
            }

            return SyntaxFactory.Identifier(identifier.LeadingTrivia, newValue, identifier.TrailingTrivia);
        }

        /// <summary>
        ///     Removes all non-digit, non-alphabetic characters. The naming convention of the first section can be specified, all
        ///     others use <see cref="NamingConvention.UpperCamelCase" />.
        ///     For example:
        ///     input = "_allo_ello"; first section = "allo"
        ///     input = "IBufferMyBuffer"; first section = "IBuffer"
        ///     input = "MY_SNAKE_CASE"; first section = "MY"
        ///     This allows us to remove things like underscores and have the individual sections they denoted in a proper
        ///     convention as well
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getFirstEntryConventioned"></param>
        /// <returns></returns>
        internal static string GetNormalizedString(string input, Func<string, string> getFirstEntryConventioned)
        {
            var sections = new List<string>();
            var tempBuffer = new StringBuilder();

            Action addSection = () =>
            {
                if (tempBuffer.Length != 0)
                {
                    sections.Add(tempBuffer.ToString());
                }

                tempBuffer.Clear();
            };

            var previousCharWasUpper = false;
            for (var i = 0; i < input.Length; i++)
            {
                var currChar = input[i];

                if (char.IsLetter(currChar) || char.IsNumber(currChar))
                {
                    var isCurrentCharUpper = char.IsUpper(currChar);
                    if (isCurrentCharUpper && !previousCharWasUpper)
                    {
                        addSection();
                    }

                    previousCharWasUpper = isCurrentCharUpper;
                    tempBuffer.Append(currChar);
                }
                else
                {
                    // We already have data to add
                    // Existing data gets added but the current character is being ignored
                    addSection();
                }
            }

            // If there is stuff remaining in the buffer, flush it as the last section
            addSection();

            // Identifiers that consist solely of underscores, e.g. _____
            if (sections.Count == 0)
            {
                return input;
            }

            var sb = new StringBuilder(getFirstEntryConventioned(sections[0]));
            for (var i = 1; i < sections.Count; i++)
            {
                sb.Append(Upper(sections[i]));
            }
            return sb.ToString();
        }

        internal static string Upper(string input)
        {
            if (input.Length == 0)
            {
                return string.Empty;
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLowerInvariant();
        }

        internal static string Lower(string input)
        {
            if (input.Length == 0)
            {
                return string.Empty;
            }

            return input.ToLowerInvariant();
        }

        internal static string IUpper(string input)
        {
            if (input.Length == 0)
            {
                return string.Empty;
            }

            if (input.Length == 1)
            {
                if (string.Equals("I", input, StringComparison.OrdinalIgnoreCase))
                {
                    return "I";
                }

                return "I" + input.ToUpperInvariant();
            }

            if (input.StartsWith("I", StringComparison.OrdinalIgnoreCase))
            {
                return "I" + char.ToUpper(input[1]) + input.Substring(2).ToLowerInvariant();
            }

            return "I" + char.ToUpper(input[0]) + input.Substring(1).ToLowerInvariant();
        }

        internal static string UnderscoreLower(string input)
        {
            if (input.Length == 0)
            {
                return string.Empty;
            }

            return "_" + Lower(input);
        }
    }
}