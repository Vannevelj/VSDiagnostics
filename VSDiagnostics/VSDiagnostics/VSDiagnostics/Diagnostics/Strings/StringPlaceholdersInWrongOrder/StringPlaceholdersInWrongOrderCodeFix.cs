using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Diagnostics.Strings.StringPlaceholdersInWrongOrder
{
    [ExportCodeFixProvider("StringPlaceHoldersInWrongOrder", LanguageNames.CSharp), Shared]
    public class StringPlaceHoldersInWrongOrderCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StringPlaceholdersInWrongOrderAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var stringFormatInvocation = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();
            context.RegisterCodeFix(
                CodeAction.Create(VSDiagnosticsResources.StringPlaceholdersInWrongOrderCodeFixTitle, x => ReOrderPlaceholdersAsync(context.Document, root, stringFormatInvocation),
                    nameof(StringPlaceholdersInWrongOrderAnalyzer)),
                diagnostic);
        }

        private static Task<Solution> ReOrderPlaceholdersAsync(Document document, SyntaxNode root, InvocationExpressionSyntax stringFormatInvocation)
        {
            var firstArgumentIsLiteral = stringFormatInvocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax;
            var formatString = ((LiteralExpressionSyntax) stringFormatInvocation.ArgumentList.Arguments[firstArgumentIsLiteral ? 0 : 1].Expression).GetText().ToString();
            var elements = StringPlaceholdersInWrongOrderHelper.GetPlaceholdersSplit(formatString);
            var matches = StringPlaceholdersInWrongOrderHelper.GetPlaceholders(formatString);

            // Here we will store a key-value pair of the old placeholder value and the new value that we associate with it
            var placeholderMapping = new Dictionary<int, int>();

            // This contains the order in which the placeholders appeared in the original format string.
            // For example if it had the string "{1} x {0} y {2} {2}" then this collection would contain the values 1-0-2.
            // You'll notice that we omitted the duplicate: we don't want to add an argument twice.
            // Typically we'd do this by using a HashSet<T> but since we can't easily retrieve an item from the HashSet<T>, 
            // we'll just check for duplicates upon inserting in the list.
            // Based on this we can then reconstruct the argument list by reordering the existing arguments.
            var placeholderIndexOrder = new List<int>();

            var amountOfPlaceholders = 0;
            var newPlaceholderValue = 0;

            var sb = new StringBuilder(elements.Length);
            for (var index = 0; index < elements.Length; index++)
            {
                // If it's a numerical value, it means we're dealing with a placeholder
                // Use Normalize() to account for formatted placeholders
                int placeholderValue;
                if (int.TryParse(StringPlaceholdersInWrongOrderHelper.Normalize(elements[index]), out placeholderValue))
                {
                    // If we already have a new value associated with this placeholder, retrieve it and add it to our result
                    if (placeholderMapping.ContainsKey(placeholderValue))
                    {
                        sb.Append(GetNewElement(matches, amountOfPlaceholders, placeholderMapping[placeholderValue]));
                    }
                    else // Otherwise use the new placeholder value and store the mapping
                    {
                        sb.Append(GetNewElement(matches, amountOfPlaceholders, newPlaceholderValue));
                        placeholderMapping.Add(placeholderValue, newPlaceholderValue);
                        newPlaceholderValue++;
                    }

                    if (!placeholderIndexOrder.Contains(placeholderValue))
                    {
                        placeholderIndexOrder.Add(placeholderValue);
                    }

                    amountOfPlaceholders++;
                }
                else
                {
                    sb.Append(elements[index]);
                }
            }
            var newFormat = sb.ToString();

            // Create a new argument for the formatting string
            var newArgument = stringFormatInvocation.ArgumentList.Arguments[firstArgumentIsLiteral ? 0 : 1].WithExpression(SyntaxFactory.ParseExpression(newFormat));

            // Create a new list for the arguments which are injected in the formatting string
            // In order to do this we iterate over the mapping which is in essence a guideline that tells us which index
            IEnumerable<ArgumentSyntax> args = firstArgumentIsLiteral
                ? new[] { newArgument }
                : new[] { stringFormatInvocation.ArgumentList.Arguments[0], newArgument };

            // Skip the formatting literal and, if applicable, the formatprovider
            var argumentsToSkip = firstArgumentIsLiteral ? 1 : 2;
            for (var index = 0; index < placeholderIndexOrder.Count; index++)
            {
                args = args.Concat(new[] { stringFormatInvocation.ArgumentList.Arguments[placeholderIndexOrder[index] + argumentsToSkip] });
            }

            // If there are less arguments in the new list compared to the old one, it means there was an unused argument
            // In that case we will loop over all the old arguments, see if they're contained in the new list and if not: add them
            // Since the variables weren't used in the first place, we can add them in whatever order we want
            // However because we are traversing from the front, they will be added in the same order as they were anyway
            if (stringFormatInvocation.ArgumentList.Arguments.Count != args.Count())
            {
                foreach (var arg in stringFormatInvocation.ArgumentList.Arguments.Skip(argumentsToSkip))
                {
                    if (!args.Contains(arg))
                    {
                        args = args.Concat(new[] { arg });
                    }
                }
            }

            var newArguments = stringFormatInvocation.ArgumentList.WithArguments(SyntaxFactory.SeparatedList(args));
            var newInvocation = stringFormatInvocation.WithArgumentList(newArguments);
            var newRoot = root.ReplaceNode(stringFormatInvocation, newInvocation);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }

        /// <summary>
        ///     Because the Regex.Split does not maintain any brackets or formatting, we have to now reconstruct the previous
        ///     placeholder but with a new index.
        /// </summary>
        private static string GetNewElement(MatchCollection matches, int oldPlaceholderIndex, int newPlaceholderValue)
        {
            var originalValue = matches[oldPlaceholderIndex].Value;
            var newValue = new StringBuilder();

            for (var index = 0; index < originalValue.Length; index++)
            {
                // Formatting detected: append everything remaining
                if (originalValue[index] == ':')
                {
                    newValue.Append(originalValue.Substring(index));
                    return newValue.ToString();
                }

                // Closing brace detected: append the remaining closing brace(s)
                if (originalValue[index] == '}')
                {
                    newValue.Append(originalValue.Substring(index));
                    return newValue.ToString();
                }

                // Opening brace detected: just add it
                if (originalValue[index] == '{')
                {
                    newValue.Append(originalValue[index]);
                }
                else
                // If it's not a formatting delimiter or open- or closing braces, it must be the actual placeholder value.
                // Replace it by appending the new value.
                {
                    newValue.Append(newPlaceholderValue);
                }
            }
            return newValue.ToString();
        }
    }
}