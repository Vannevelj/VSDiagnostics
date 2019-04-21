using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.DateTimeNow;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class DateTimeNowTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new DateTimeNowAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new DateTimeNowCodeFix();

        [TestMethod]
        public void DateTimeNow_Now()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var date = DateTime.Now;
        }
    }
}";

            var result = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var date = DateTime.UtcNow;
        }
    }
}";

            VerifyDiagnostic(original, "Use DateTime.UtcNow to get a consistent value");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void DateTimeNow_UtcNow()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var date = DateTime.UtcNow;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DateTimeNow_Now_Expression()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Console.WriteLine(DateTime.Now);
        }
    }
}";

            var result = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Console.WriteLine(DateTime.UtcNow);
        }
    }
}";

            VerifyDiagnostic(original, "Use DateTime.UtcNow to get a consistent value");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void DateTimeNow_FullName()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var date = System.DateTime.Now;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var date = System.DateTime.UtcNow;
        }
    }
}";

            VerifyDiagnostic(original, "Use DateTime.UtcNow to get a consistent value");
            VerifyFix(original, result);
        }
    }
}
