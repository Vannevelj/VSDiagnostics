using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.CatchNullReferenceException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class CatchNullReferenceExceptionAnalyzerTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CatchNullReferenceExceptionAnalyzer();

        [TestMethod]
        public void CatchNullReferenceExceptionAnalyzer_WithNullReferenceCatchClause_InvokesWarning()
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
                catch(NullReferenceException e)
                {

                }
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = CatchNullReferenceExceptionAnalyzer.DiagnosticId,
                Message = CatchNullReferenceExceptionAnalyzer.Message,
                Severity = CatchNullReferenceExceptionAnalyzer.Severity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 22)
                }
            };

            VerifyDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void CatchNullReferenceExceptionAnalyzer_WithoutNullReferenceCatchClause_DoesNotInvokeWarning()
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

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void CatchNullReferenceExceptionAnalyzer_WithEmptyCatchClause_DoesNotInvokeWarning()
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

            VerifyDiagnostic(test);
        }
    }
}