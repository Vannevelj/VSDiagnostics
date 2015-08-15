using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
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
            context.RegisterCodeFix(CodeAction.Create("Re-order placeholders", x => ReOrderPlaceholdersAsync(context.Document, root, stringFormatInvocation), nameof(StringPlaceholdersInWrongOrderAnalyzer)),
                diagnostic);
        }

        private static Task<Solution> ReOrderPlaceholdersAsync(Document document, SyntaxNode root, InvocationExpressionSyntax stringFormatInvocation)
        {
            var firstArgumentIsLiteral = stringFormatInvocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax;
            var formatString = ((LiteralExpressionSyntax) stringFormatInvocation.ArgumentList.Arguments[firstArgumentIsLiteral ? 0 : 1].Expression).GetText().ToString();
            var elements = StringPlaceholdersInWrongOrderHelper.GetPlaceholdersSplit(formatString);
            var matches = StringPlaceholdersInWrongOrderHelper.GetPlaceholders(formatString);

            // From all our elements, get the ones that represent an integer and get the lowest one. 
            // This will give us our starting index
            var newPlaceholderIndex = elements.Min(x =>
            {
                int intValue;
                if (!int.TryParse(x, out intValue))
                {
                    intValue = int.MaxValue;
                }

                return intValue;
            });
            var placeholderMapping = new Dictionary<int, int>();
            var placeholderIndex = 0;

            Func<int, int, string> getNewElement = (oldPlaceholderIndex, newPlaceholderValue) =>
            {
                var originalValue = matches[oldPlaceholderIndex].Value;
                var newValue = new StringBuilder();

                for (var index = 0; index < originalValue.Length; index++)
                {
                    if (originalValue[index] == ':')
                    {
                        newValue.Append(originalValue.Substring(index));
                        return newValue.ToString();
                    }
                    if (originalValue[index] == '{')
                    {
                        newValue.Append(originalValue[index]);
                    }
                    else if (originalValue[index] == '}')
                    {
                        newValue.Append(originalValue.Substring(index));
                        return newValue.ToString();
                    }
                    else
                    {
                        newValue.Append(newPlaceholderValue);
                    }
                }
                return newValue.ToString();
            };

            var sb = new StringBuilder(elements.Length);
            for (var index = 0; index < elements.Length; index++)
            {
                int placeholderValue;
                if (int.TryParse(elements[index], out placeholderValue))
                {
                    if (placeholderMapping.ContainsKey(placeholderValue))
                    {
                        sb.Append(getNewElement(placeholderIndex, placeholderMapping[placeholderValue]));
                    }
                    else
                    {
                        sb.Append(getNewElement(placeholderIndex, newPlaceholderIndex));
                        placeholderMapping.Add(placeholderValue, newPlaceholderIndex);
                        newPlaceholderIndex++;
                    }

                    placeholderIndex++;
                }
                else
                {
                    sb.Append(elements[index]);
                }
            }
            var newFormat = sb.ToString();

            var newArgument = stringFormatInvocation.ArgumentList.Arguments[firstArgumentIsLiteral ? 0 : 1].WithExpression(SyntaxFactory.ParseExpression(newFormat));
            var newArguments = stringFormatInvocation.ArgumentList.WithArguments(SyntaxFactory.SeparatedList(new[] { newArgument }.Concat(stringFormatInvocation.ArgumentList.Arguments.Skip(1).ToArray())));
            var newInvocation = stringFormatInvocation.WithArgumentList(newArguments);
            var newRoot = root.ReplaceNode(stringFormatInvocation, newInvocation);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}