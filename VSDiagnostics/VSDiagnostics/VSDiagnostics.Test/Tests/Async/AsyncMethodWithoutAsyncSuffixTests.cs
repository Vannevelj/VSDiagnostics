using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class AsyncMethodWithoutAsyncSuffixTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider => new AsyncMethodWithoutAsyncSuffixCodeFix();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AsyncMethodWithoutAsyncSuffixAnalyzer();

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithAsyncMethodAndNoSuffix()
        {
            var original = @"
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
            Task MethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithAsyncMethodAndSuffix()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            Task MethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithSynchronousMethodAndNoSuffix()
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
        public void AsyncMethodWithoutAsyncSuffix_WithInheritance()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethod();
        }

        class MyClass : IMyInterface
        {
            public Task MyMethod()
            {
                return Task.CompletedTask;
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
            Task MyMethodAsync();
        }

        class MyClass : IMyInterface
        {
            public Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }
    }
}