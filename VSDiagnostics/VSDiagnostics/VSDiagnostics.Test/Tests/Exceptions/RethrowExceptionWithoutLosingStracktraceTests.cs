using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.RethrowExceptionWithoutLosingStacktrace;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class RethrowExceptionWithoutLosingStracktraceTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RethrowExceptionWithoutLosingStacktraceAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new RethrowExceptionWithoutLosingStacktraceCodeFix();

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_WithRethrowArgument_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception e)
            {
                throw;
            }
        }
    }
}";

            VerifyDiagnostic(original, RethrowExceptionWithoutLosingStacktraceAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result, allowNewCompilerDiagnostics: true); // Removing the argument will remove all usages of the e parameter. This will cause a CS0168 warning.
        }

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_ThrowsANewException_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception e)
            {
                throw new Exception(""test"", e);
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_WithRethrows_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception e)
            {
                throw;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_ThrowingANewPredefinedException_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception e)
            {
                var newException = new Exception(""test"", e);
                throw newException;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_WithThrowStatementOutsideCatchClause_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            throw new Exception();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_WithRethrowArgument_AndNoIdentifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception)
            {
                var e = new Exception();
                throw e;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RethrowExceptionWithoutLosingStracktrace_WithRethrow_AndNoIdentifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            try
            {

            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}