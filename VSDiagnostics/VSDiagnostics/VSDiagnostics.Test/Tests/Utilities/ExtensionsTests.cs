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

            AssertMethodIsAsync(source, expectedAsync: true);
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

            AssertMethodIsAsync(source, expectedAsync: true);
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

            AssertMethodIsAsync(source, expectedAsync: true);
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

            AssertMethodIsAsync(source, expectedAsync: true);
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

            AssertMethodIsAsync(source, expectedAsync: false);
        }

        [TestMethod]
        public void IsDefinedInAncestor_OnlyOneDefinition()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass : IMyInterface
        {
            public void MyMethod()
            {
            }
        }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var method = GetMethodNodes(tree).Single();
            var methodSymbol = GetSemanticModel(tree).GetDeclaredSymbol(method);

            Assert.IsFalse(methodSymbol.IsDefinedInAncestor());
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInInterface_WithImplementedMember()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            void MyMethod();
        }

        class MyClass : IMyInterface
        {
            public void MyMethod()
            {
            }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithOverriddenMember_FromAbstractMethod()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract void MyMethod();
        }

        class MyClass : BaseClass
        {
            public override void MyMethod()
            {
            }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithOverriddenMember_FromVirtualMethod()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual void MyMethod()
            {
            }
        }

        class MyClass : BaseClass
        {
            public override void MyMethod()
            {
            }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithLongerInheritanceStructure_WithClasses()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseBaseClass
        {
	        public virtual void MyMethod()
	        {
	        }
        }

        class BaseClass : BaseBaseClass
        {
	        public override void MyMethod()
	        {
	        }
        }

        class MyClass : BaseClass
        {
	        public override void MyMethod()
	        {
	        }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithLongerInheritanceStructure_WithInterfaces()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IBaseInterface
        {
            void MyMethod();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : IMyInterface
        {
	        public void MyMethod()
	        {
	        }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithInterfacesAndClasses()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
	        public abstract void MyMethod();
        }

        interface IBaseInterface
        {
	        void MyMethod();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : BaseClass, IMyInterface
        {
	        public override void MyMethod()
	        {
	        }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_WithOverriddenMember_WithMultipleInterfaces()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            void MyMethod();
        }

        interface IAnotherInterface
        {
            void MyMethod();
        }

        class MyClass : IMyInterface, IAnotherInterface
        {
	        public void MyMethod()
	        {
	        }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithMultipleAbstractClasses()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
	        public abstract void MyMethod();
        }

        abstract class BaseClass : OtherBaseClass
        {
	        
        }

        class MyClass : BaseClass
        {
	        public override void MyMethod()
	        {
	        }
        }
    }";

            AssertMethodIsDefinedInAncestor(source);
        }

        /// <summary>
        ///     This should not display a warning because hidden methods are not considered the same by Roslyn.
        ///     You will also notice that when you rename a hidden method in VS2015, it will only rename one of the two methods.
        ///     Therefore, in this scenario, we won't associate the two methods as one and the same, otherwise analyzers would
        ///     hide the base member, and not show a diagnostic for that one.
        /// </summary>
        [TestMethod]
        public void IsDefinedInAncestor_DefinedInBaseClass_WithHiddenMember()
        {
            var source = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
            public virtual void MyMethod()
	        {
	        }
        }

        abstract class BaseClass : OtherBaseClass
        {
            public new void MyMethod()
	        {
	        }
        }
    }";

            var tree = CSharpSyntaxTree.ParseText(source);
            var method = GetMethodNodes(tree).Last();
            var methodSymbol = GetSemanticModel(tree).GetDeclaredSymbol(method);

            Assert.IsFalse(methodSymbol.IsDefinedInAncestor());
        }

        private static void AssertMethodIsAsync(string source, bool expectedAsync)
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var method = GetMethodNodes(tree).First();
            var methodSymbol = GetSemanticModel(tree).GetDeclaredSymbol(method);

            Assert.AreEqual(expectedAsync, methodSymbol.IsAsync());
        }

        private static void AssertMethodIsDefinedInAncestor(string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var method = GetMethodNodes(tree).Last();
            var methodSymbol = GetSemanticModel(tree).GetDeclaredSymbol(method);

            Assert.IsTrue(methodSymbol.IsDefinedInAncestor());
        }

        private static IEnumerable<MethodDeclarationSyntax> GetMethodNodes(SyntaxTree tree)
        {
            return tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
        }

        private static SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { tree }, new[] { mscorlib });
            return compilation.GetSemanticModel(tree);
        }
    }
}