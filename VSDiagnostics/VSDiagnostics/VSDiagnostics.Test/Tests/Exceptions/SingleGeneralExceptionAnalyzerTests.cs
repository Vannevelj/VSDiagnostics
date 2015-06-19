using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.SingleGeneralException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class SingleGeneralExceptionAnalyzerTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SingleGeneralExceptionAnalyzer();

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

            VerifyDiagnostic(test, SingleGeneralExceptionAnalyzer.Rule.MessageFormat.ToString());
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

            VerifyDiagnostic(test);
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

            VerifyDiagnostic(test);
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

            VerifyDiagnostic(test);
        }
    }
}