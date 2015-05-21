using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.Exceptions.EmptyArgumentException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class EmptyArgumentExceptionAnalyzerTests : DiagnosticVerifier
    {
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = EmptyArgumentExceptionAnalyzer.DiagnosticId,
                Message = EmptyArgumentExceptionAnalyzer.Message,
                Severity = EmptyArgumentExceptionAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = EmptyArgumentExceptionAnalyzer.DiagnosticId,
                Message = EmptyArgumentExceptionAnalyzer.Message,
                Severity = EmptyArgumentExceptionAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
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
            VerifyCSharpDiagnostic(test);
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

            VerifyCSharpDiagnostic(test);
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

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EmptyArgumentExceptionAnalyzer();
        }
    }
}