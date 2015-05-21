using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.Exceptions.CatchNullReferenceException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class CatchNullReferenceExceptionAnalyzerTests : DiagnosticVerifier
    {
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
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

            VerifyCSharpDiagnostic(test);
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

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CatchNullReferenceExceptionAnalyzer();
        }
    }
}