using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Tests.RemoveTestSuffix;

namespace VSDiagnostics.Test.Tests.Tests
{
    [TestClass]
    public class RemoveTestSuffixTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RemoveTestSuffixAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new RemoveTestSuffixCodeFix();

        [TestMethod]
        public void RemoveTestSuffix_TestMethodMethodEndsWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [TestMethod]
        public void MethodTest()
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [TestMethod]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original, string.Format(RemoveTestSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodTest"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RemoveTestSuffix_TestMethodMethodDoesNotEndWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [TestMethod]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RemoveTestSuffix_NonTestMethodEndsWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public void MethodTest()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RemoveTestSuffix_NonTestMethodDoesNotEndWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }



        [TestMethod]
        public void RemoveTestSuffix_AttributedMethodEndsWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        [Obsolete]
        public void MethodTest()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RemoveTestSuffix_FactMethodEndsWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [Fact]
        public void MethodTest()
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [Fact]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original, string.Format(RemoveTestSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodTest"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RemoveTestSuffix_FactMethodDoesNotEndWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [Fact]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RemoveTestSuffix_TestMethodEndsWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [Test]
        public void MethodTest()
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [Test]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original, string.Format(RemoveTestSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodTest"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RemoveTestSuffix_TestMethodDoesNotEndWithTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [Test]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RemoveTestSuffix_TestMethodDoesNotEndWithTest_UpdatesReferences()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        void Foo()
        {
            MethodTest();
        }

        [Test]
        public void MethodTest()
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        void Foo()
        {
            Method();
        }

        [Test]
        public void Method()
        {
        }
    }
}";

            VerifyDiagnostic(original, string.Format(RemoveTestSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodTest"));
            VerifyFix(original, result);
        }
    }
}