using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.EmptyArgumentException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class EmptyArgumentExceptionAnalyzerTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EmptyArgumentExceptionAnalyzer();

        [TestMethod]
        public void EmptyArgumentExceptionAnalyzer_WithEmptyArgument_InvokesWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentException();
            }
        }
    }";

            VerifyDiagnostic(test, EmptyArgumentExceptionAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void EmptyArgumentExceptionAnalyzer_WithEmptyNullArgument_InvokesWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentNullException();
            }
        }
    }";

            VerifyDiagnostic(test, EmptyArgumentExceptionAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void EmptyArgumentExceptionAnalyzer_WithArgument_DoesNotInvokeWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentException(""input"");
            }
        }
    }";
            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void EmptyArgumentExceptionAnalyzer_WithDumbRethrowStatement_DoesNotInvokeWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                try { }
                catch (ArgumentException e) { throw e; }
            }
        }
    }";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void EmptyArgumentExceptionAnalyzer_WithRethrowStatement_DoesNotInvokeWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                try { }
                catch (ArgumentException e) { throw; }
            }
        }
    }";

            VerifyDiagnostic(test);
        }
    }
}