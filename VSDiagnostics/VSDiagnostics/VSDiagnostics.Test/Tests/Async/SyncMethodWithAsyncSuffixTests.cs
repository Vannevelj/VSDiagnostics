using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.SyncMethodWithAsyncSuffix;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class SyncMethodWithAsyncSuffixTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider => new SyncMethodWithAsyncSuffixCodeFix();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SyncMethodWithAsyncSuffixAnalyzer();

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithSynchronousMethodAndNoSuffix()
        {
            var original = @"
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
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithSynchronousMethodAndSuffix()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void MethodAsync()
            {
            }
        }
    }";

            var result = @"
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
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithAsyncMethodAndSuffix()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task MethodAsync()
            {
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithInheritance()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            void MyMethodAsync();
        }

        class MyClass : IMyInterface
        {
            public void MyMethodAsync()
            {
            }
        }
    }";

            var result = @"
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

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }
    }
}