using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.Exceptions.SingleGeneralException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class SingleGeneralExceptionAnalyzerTests : DiagnosticVerifier
    {
        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithSingleGeneralException_InvokesWarning()
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
                try 
                {
                }
                catch(Exception e)
                {

                }
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = SingleGeneralExceptionAnalyzer.DiagnosticId,
                Message = SingleGeneralExceptionAnalyzer.Message,
                Severity = SingleGeneralExceptionAnalyzer.Severity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithSingleSpecificException_DoesNotInvokeWarning()
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
                try 
                {
                }
                catch(ArgumentException e)
                {

                }
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithoutNamedCatchClauses_DoesNotInvokeWarning()
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
                try 
                {
                }
                catch
                {

                }
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithMultipleCatchClauses_DoesNotInvokeWarning()
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
                try 
                {
                }
                catch(ArgumentException e)
                {

                }
                catch(Exception e)
                {

                }
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SingleGeneralExceptionAnalyzer();
        }
    }
}