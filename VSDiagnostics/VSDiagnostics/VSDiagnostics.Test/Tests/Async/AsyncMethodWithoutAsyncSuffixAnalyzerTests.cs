using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.Async;
using VSDiagnostics.Diagnostics.Exceptions.EmptyArgumentExceptionAnalyzer;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class AsyncMethodWithoutAsyncSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffixAnalyzer_WithAsyncKeywordAndNoSuffix_InvokesWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task Method()
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffixAnalyzer_WithAsyncKeywordAndSuffix_DoesNotDisplayWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task MethodAsync()
            {
                
            }
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffixAnalyzer_WithoutAsyncKeywordAndSuffix_DoesNotDisplayWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                
            }
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AsyncMethodWithoutAsyncSuffixAnalyzer();
        }
    }
}