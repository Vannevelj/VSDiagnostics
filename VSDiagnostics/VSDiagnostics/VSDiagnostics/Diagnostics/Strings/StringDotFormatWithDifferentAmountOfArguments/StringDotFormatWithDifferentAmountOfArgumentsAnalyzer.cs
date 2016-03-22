using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VSDiagnostics.Utilities;
// ReSharper disable LoopCanBeConvertedToQuery

namespace VSDiagnostics.Diagnostics.Strings.StringDotFormatWithDifferentAmountOfArguments
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringDotFormatWithDifferentAmountOfArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;
        private static readonly string Category = VSDiagnosticsResources.StringsCategory;
        private static readonly string Message = VSDiagnosticsResources.StringDotFormatWithDifferentAmountOfArgumentsMessage;
        private static readonly string Title = VSDiagnosticsResources.StringDotFormatWithDifferentAmountOfArgumentsTitle;

        internal static DiagnosticDescriptor Rule =>
            new DiagnosticDescriptor(DiagnosticId.StringDotFormatWithDifferentAmountOfArguments, Title, Message, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax) context.Node;
            if (invocation.ArgumentList == null)
            {
                return;
            }

            // Get the format string
            // This corresponds to the argument passed to the parameter with name 'format'
            var invokedMethod = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = invokedMethod.Symbol as IMethodSymbol;

            if (methodSymbol == null)
            {
                return;
            }

            // Verify we're dealing with a call to a method that accepts a variable named 'format' and a object, params object[] or a plain object[]
            // params object[] and object[] can both be verified by looking for the latter
            // This allows us to support similar calls like Console.WriteLine("{0}", "test") as well which carry an implicit string.Format
            IParameterSymbol formatParam = null;
            foreach (var parameter in methodSymbol.Parameters)
            {
                if (parameter.Name == "format")
                {
                    formatParam = parameter;
                    break;
                }
            }

            if (formatParam == null)
            {
                return;
            }

            var formatIndex = formatParam.Ordinal;
            var formatParameters = new List<IParameterSymbol>();// methodSymbol.Parameters.Skip(formatIndex + 1).ToArray();
            for (var i = formatIndex + 1; i < methodSymbol.Parameters.Length; i++)
            {
                formatParameters.Add(methodSymbol.Parameters[i]);
            }

            // If the method definition doesn't contain any parameter to pass format arguments, we ignore it
            if (!formatParameters.NonLinqAny())
            {
                return;
            }

            var symbolsAreNotArraysOrObjects = true;
            foreach (var symbol in formatParameters)
            {
                if (symbol.Type.Kind != SymbolKind.ArrayType || ((IArrayTypeSymbol) symbol.Type).ElementType.SpecialType != SpecialType.System_Object)
                {
                    symbolsAreNotArraysOrObjects = false;
                    break;
                }
            }
            var hasObjectArray = formatParameters.Count == 1 && symbolsAreNotArraysOrObjects;

            var hasObject = true;
            foreach (var symbol in formatParameters)
            {
                if (symbol.Type.SpecialType != SpecialType.System_Object)
                {
                    hasObject = false;
                    break;
                }
            }

            if (!(hasObject || hasObjectArray))
            {
                return;
            }

            // In case less arguments are passed in than the format definition, we escape
            // This can occur when dealing with optional arguments
            // Definition: string MyMethod(string format = null, object[] args = null) { }
            // Invocation: MyMethod();
            // Result: ArgumentOutOfRangeException when trying to access the format argument based on the format parameter's index
            if (invocation.ArgumentList.Arguments.Count <= formatIndex)
            {
                return;
            }

            var formatExpression = invocation.ArgumentList.Arguments[formatIndex].Expression;
            var formatString = context.SemanticModel.GetConstantValue(formatExpression);
            if (!formatString.HasValue)
            {
                return;
            }

            // Get the total amount of arguments passed in for the format
            // If the first one is the literal (aka: the format specified) then every other argument is an argument to the format
            // If not, it means the first one is the CultureInfo, the second is the format and all others are format arguments
            // We also have to check whether or not the arguments are passed in through an explicit array or whether they use the params syntax
            var formatArguments = new List<ArgumentSyntax>();
            for (var i = formatIndex + 1; i < invocation.ArgumentList.Arguments.Count; i++)
            {
                formatArguments.Add(invocation.ArgumentList.Arguments[i]);
            }

            var amountOfFormatArguments = formatArguments.Count;

            if (amountOfFormatArguments == 1)
            {
                var argumentType = context.SemanticModel.GetTypeInfo(formatArguments[0].Expression);
                if (argumentType.Type == null)
                {
                    return;
                }

                // Inline array creation à la string.Format("{0}", new object[] { "test" })
                if (argumentType.Type.TypeKind == TypeKind.Array)
                {
                    // We check for an invocation first to account for the scenario where you have both an invocation and an array initializer
                    // Think about something like this: string.Format(""{0}{1}{2}"", new[] { 1 }.Concat(new[] {2}).ToArray());
                    var methodInvocation = formatArguments[0].DescendantNodes().NonLinqOfType<InvocationExpressionSyntax>(SyntaxKind.InvocationExpression).NonLinqFirstOrDefault();
                    if (methodInvocation != null)
                    {
                        // We don't handle method calls that return an array in the case of a single argument
                        return;
                    }

                    InitializerExpressionSyntax inlineArrayCreation = null;
                    foreach (var argument in formatArguments[0].DescendantNodes())
                    {
                        var argumentAsInitializerExpressionSyntax = argument as InitializerExpressionSyntax;
                        if (argumentAsInitializerExpressionSyntax != null)
                        {
                            inlineArrayCreation = argumentAsInitializerExpressionSyntax;
                        }
                    }

                    if (inlineArrayCreation != null)
                    {
                        amountOfFormatArguments = inlineArrayCreation.Expressions.Count;
                        goto placeholderVerification;
                    }

                    // If we got here it means the arguments are passed in through an identifier which resolves to an array 
                    // aka: referencing a variable/field that is of type T[]
                    // We cannot reliably get the amount of arguments if it's a method
                    // We could get them when it's a field/variable/property but that takes some more work and thinking about it
                    // This is tracked in workitem https://github.com/Vannevelj/VSDiagnostics/issues/330
                    if (hasObjectArray)
                    {
                        return;
                    }
                }
            }

            placeholderVerification:
            // Get the placeholders we use stripped off their format specifier, get the highest value 
            // and verify that this value + 1 (to account for 0-based indexing) is not greater than the amount of placeholder arguments
            var placeholders = new List<int>();

            foreach (Match placeholder in PlaceholderHelpers.GetPlaceholders((string) formatString.Value))
            {
                placeholders.Add(int.Parse(PlaceholderHelpers.GetPlaceholderIndex(placeholder.Value)));
            }

            if (!placeholders.NonLinqAny())
            {
                return;
            }

            var highestPlaceholder = int.MinValue;
            foreach (var placeholder in placeholders)
            {
                highestPlaceholder = Math.Max(highestPlaceholder, placeholder);
            }

            if (highestPlaceholder + 1 > amountOfFormatArguments)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, formatExpression.GetLocation()));
            }
        }
    }
}