using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Tests.RemoveTestSuffix;

namespace VSDiagnostics.Test.Tests.Tests
{
    [TestClass]
    public class RemoveTestSuffixTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RemoveTestSuffixAnalyzer();

        [TestMethod]
        public void RemoveTestSuffix_TestMethodEndsWithTest_InvokesWarning()
        {
            var original = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            VerifyDiagnostic(original, string.Format(RemoveTestSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodTest"));
        }

        [TestMethod]
        public void RemoveTestSuffix_TestMethodDoesNotEndWithTest_DoesNotInvokeWarning()
        {
            var original = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void RemoveTestSuffix_NonTestMethodEndsWithTest_DoesNotInvokeWarning()
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
        public void RemoveTestSuffix_NonTestMethodDoesNotEndWithTest_DoesNotInvokeWarning()
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
    }
}