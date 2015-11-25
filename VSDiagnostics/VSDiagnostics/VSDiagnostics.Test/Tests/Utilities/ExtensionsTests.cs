using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSDiagnostics.Utilities;

namespace VSDiagnostics.Test.Tests.Utilities
{
    [TestClass]
    public class ExtensionsTests
    {
        // See https://github.com/Vannevelj/VSDiagnostics/issues/205
        [TestMethod]
        [Timeout(10 * 1000)]
        public void InheritsFrom_WithIrrelevantType()
        {
            var source = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                var x = new string();
            }
        }
    }";
            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);
            
            var root = tree.GetRoot();
            var objectCreationExpression = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().First();
            var typeSymbol = semanticModel.GetSymbolInfo(objectCreationExpression.Type);
            Assert.IsFalse(typeSymbol.Symbol.InheritsFrom(typeof(Exception)));
        }

        [TestMethod]
        public void InheritsFrom_WithRelevantType()
        {
            var source = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                var x = new ArgumentNullException();
            }
        }
    }";
            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var objectCreationExpression = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().First();
            var typeSymbol = semanticModel.GetSymbolInfo(objectCreationExpression.Type);
            Assert.IsTrue(typeSymbol.Symbol.InheritsFrom(typeof(ArgumentException)));
        }

        [TestMethod]
        public void InheritsFrom_WithRelevantType_WithExceptionType()
        {
            var source = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                var x = new ArgumentNullException();
            }
        }
    }";
            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var objectCreationExpression = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().First();
            var typeSymbol = semanticModel.GetSymbolInfo(objectCreationExpression.Type);
            Assert.IsTrue(typeSymbol.Symbol.InheritsFrom(typeof(Exception)));
        }

        [TestMethod]
        public void IsAsync_WithAsyncKeywordAndTaskReturnType()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            async Task Method()
            {
            }
        }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
            Assert.IsTrue(methodSymbol.IsAsync());
        }

        [TestMethod]
        public void IsAsync_WithTaskReturnType()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            Task Method()
            {
                return Task.CompletedTask;
            }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
            Assert.IsTrue(methodSymbol.IsAsync());
        }

        [TestMethod]
        public void IsAsync_WithGenericTaskReturnType()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            Task<int> Method()
            {
                return Task.FromResult(0);
            }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
            Assert.IsTrue(methodSymbol.IsAsync());
        }

        [TestMethod]
        public void IsAsync_WithAsyncKeywordAndVoidReturnType()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            async void Method()
            {
            }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
            Assert.IsTrue(methodSymbol.IsAsync());
        }

        [TestMethod]
        public void IsAsync_WithVoidReturnType()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
            }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
            Assert.IsFalse(methodSymbol.IsAsync());
        }
    }
}