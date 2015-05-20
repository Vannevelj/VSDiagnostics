using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix;
using VSDiagnostics.Diagnostics.Exceptions.EmptyArgumentException;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class AsyncMethodWithoutAsyncSuffixAnalyzerTests : CodeFixVerifier
    {
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
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.DiagnosticId,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Message, "Method"),
                Severity = EmptyArgumentExceptionAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AsyncMethodWithoutAsyncSuffixAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AsyncMethodWithoutAsyncSuffixCodeFix();
        }
    }
}