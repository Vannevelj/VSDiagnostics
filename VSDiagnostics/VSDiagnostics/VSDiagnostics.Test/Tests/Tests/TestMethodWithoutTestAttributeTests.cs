using System.Data;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Tests.TestMethodWithoutTestAttribute;

namespace VSDiagnostics.Test.Tests.Tests
{
    [TestClass]
    public class TestMethodWithoutTestAttributeTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new TestMethodWithoutTestAttributeAnalyzer();

        [TestMethod]
        public void TestMethodWithoutTestAttribute_MSTest()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        public void MyMethod()
        {
        }
    }
}";

            VerifyDiagnostic(original, "Method MyMethod might be mistakingly missing a test attribute");
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_NUnit()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestFixture]
    class MyClass
    {
        public void MyMethod()
        {
        }
    }
}";

            VerifyDiagnostic(original, "Method MyMethod might be mistakingly missing a test attribute");
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_XUnit_NoOtherMethodsWithAttribute()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public void MyMethod()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_XUnit_OtherMethodWithAttribute()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public void MyMethod()
        {
        }

        [Fact]
        public void MyOtherMethod()
        {
        }
    }
}";

            VerifyDiagnostic(original, "Method MyMethod might be mistakingly missing a test attribute");
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_struct()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    struct MyStruct
    {
        public void MyMethod()
        {
        }
    }
}";

            VerifyDiagnostic(original, "Method MyMethod might be mistakingly missing a test attribute");
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_TaskReturn()
        {
            var original = @"
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        public async Task MyMethod()
        {
            await Task.Delay(0);
        }
    }
}";

            VerifyDiagnostic(original, "Method MyMethod might be mistakingly missing a test attribute");
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_TaskTReturn()
        {
            var original = @"
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        public Task<int> MyMethod()
        {
            return Task.FromResult(5);
        }
    }
}";

            VerifyDiagnostic(original, "Method MyMethod might be mistakingly missing a test attribute");
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_OtherReturnType()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        public int MyMethod()
        {
            return 5;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TestMethodWithoutTestAttribute_OtherAttribute()
        {
            var original = @"
namespace ConsoleApplication1
{
    [TestClass]
    class MyClass
    {
        [SomethingElse]
        public int MyMethod()
        {
            return 5;
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}
