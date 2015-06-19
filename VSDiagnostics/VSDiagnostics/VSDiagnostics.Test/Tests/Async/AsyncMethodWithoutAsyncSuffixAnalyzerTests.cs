using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix;
using VSDiagnostics.Diagnostics.Exceptions.EmptyArgumentException;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class AsyncMethodWithoutAsyncSuffixAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider => new AsyncMethodWithoutAsyncSuffixCodeFix();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AsyncMethodWithoutAsyncSuffixAnalyzer();

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffixAnalyzer_WithAsyncKeywordAndNoSuffix_InvokesWarning()
        {
            var original = @"
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

            var result = @"
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "Method"),
                Severity = EmptyArgumentExceptionAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 24)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffixAnalyzer_WithAsyncKeywordAndSuffix_DoesNotDisplayWarning()
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
        public void AsyncMethodWithoutAsyncSuffixAnalyzer_WithoutAsyncKeywordAndSuffix_DoesNotDisplayWarning()
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
    }
}