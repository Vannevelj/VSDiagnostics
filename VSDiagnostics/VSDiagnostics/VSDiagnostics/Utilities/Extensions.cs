using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSDiagnostics.Utilities
{
    // TODO: after null checks, throw argumentnullexceptions instead of returning false
    public static class Extensions
    {
        private static readonly Dictionary<string, string> AliasMapping = new Dictionary<string, string>
        {
            { nameof(Int16), "short" },
            { nameof(Int32), "int" },
            { nameof(Int64), "long" },
            { nameof(UInt16), "ushort" },
            { nameof(UInt32), "uint" },
            { nameof(UInt64), "ulong" },
            { nameof(Object), "object" },
            { nameof(Byte), "byte" },
            { nameof(SByte), "sbyte" },
            { nameof(Char), "char" },
            { nameof(Boolean), "bool" },
            { nameof(Single), "float" },
            { nameof(Double), "double" },
            { nameof(Decimal), "decimal" },
            { nameof(String), "string" }
        };


        public static bool ImplementsInterface(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel,
                                               Type interfaceType)
        {
            if (classDeclaration == null)
            {
                return false;
            }

            var declaredSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            return declaredSymbol != null &&
                   (declaredSymbol.Interfaces.Any(i => i.MetadataName == interfaceType.Name) ||
                    declaredSymbol.BaseType.MetadataName == typeof(INotifyPropertyChanged).Name);

            // For some peculiar reason, "class Foo : INotifyPropertyChanged" doesn't have any interfaces,
            // But "class Foo : IFoo, INotifyPropertyChanged" has two.  "IFoo" is an interface defined by me.
            // However, the BaseType for the first is the "INotifyPropertyChanged" symbol.
            // Also, "class Foo : INotifyPropertyChanged, IFoo" has just one - "IFoo",
            // But the BaseType again is "INotifyPropertyChanged".
        }

        public static bool InheritsFrom(this ISymbol typeSymbol, Type type)
        {
            if (typeSymbol == null || type == null)
            {
                return false;
            }

            var baseType = typeSymbol;
            while (baseType != null && baseType.MetadataName != typeof(object).Name &&
                   baseType.MetadataName != typeof(ValueType).Name)
            {
                if (baseType.MetadataName == type.Name)
                {
                    return true;
                }
                baseType = ((ITypeSymbol) baseType).BaseType;
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

        public static string ToAlias(this string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string foundValue;
            if (AliasMapping.TryGetValue(type, out foundValue))
            {
                return foundValue;
            }

            throw new ArgumentException("Could not find the type specified", nameof(type));
        }

        public static bool HasAlias(this string type, out string alias)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return AliasMapping.TryGetValue(type, out alias);
        }

        /// <summary>
        ///     Determines whether or not the specified <see cref="IMethodSymbol" /> is the symbol of an asynchronous method. This
        ///     can be a method declared as async (e.g. returning <see cref="Task" /> or <see cref="Task{TResult}" />), or a method
        ///     with an async implementation (using the <code>async</code> keyword).
        /// </summary>
        public static bool IsAsync(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.IsAsync
                   || methodSymbol.ReturnType.MetadataName == typeof(Task).Name
                   || methodSymbol.ReturnType.MetadataName == typeof(Task<>).Name;
        }

        public static bool IsDefinedInAncestor(this IMethodSymbol methodSymbol)
        {
            var containingType = methodSymbol?.ContainingType;
            if (containingType == null)
            {
                return false;
            }

            var interfaces = containingType.AllInterfaces;
            foreach (var @interface in interfaces)
            {
                var interfaceMethods =
                    @interface.GetMembers().Select(containingType.FindImplementationForInterfaceMember).Where(x => x != null);

                if (interfaceMethods.Any(method => method.Equals(methodSymbol)))
                {
                    return true;
                }
            }

            var baseType = containingType.BaseType;
            while (baseType != null)
            {
                var baseMethods = baseType.GetMembers().OfType<IMethodSymbol>();
                if (baseMethods.Any(method => method.Equals(methodSymbol.OverriddenMethod)))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }

            return false;
        }

        // TODO: tests
        // NOTE: string.Format() vs Format() (current/external type)
        public static bool IsAnInvocationOf(this InvocationExpressionSyntax invocation, Type type, string method,
                                            SemanticModel semanticModel)
        {
            var invokedMethod = semanticModel.GetSymbolInfo(invocation);
            var invokedType = invokedMethod.Symbol?.ContainingType;
            if (invokedType == null)
            {
                return false;
            }

            return invokedType.MetadataName == type.Name &&
                   invokedMethod.Symbol.MetadataName == method;
        }

        // TODO: tests
        public static bool IsNameofInvocation(this InvocationExpressionSyntax invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            var identifier = invocation.Expression.DescendantNodesAndSelf()
                                       .OfType<IdentifierNameSyntax>()
                                       .FirstOrDefault();

            return identifier != null && identifier.Identifier.ValueText == "nameof";
        }
    }
}