using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.SyncMethodWithSyncSuffix;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class SyncMethodWithAsyncSuffixTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider => new SyncMethodWithAsyncSuffixCodeFix();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SyncMethodWithAsyncSuffixAnalyzer();

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithoutAsyncKeywordAndSuffix_InvokesWarning()
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
    }
}