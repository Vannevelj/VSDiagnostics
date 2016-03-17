using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.SingleGeneralException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class SingleGeneralExceptionTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SingleGeneralExceptionAnalyzer();

        [TestMethod]
        public void SingleGeneralException_WithSingleGeneralException()
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
        public void SingleGeneralException_WithSingleSpecificException()
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
        public void SingleGeneralException_WithoutNamedCatchClauses()
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
        public void SingleGeneralException_WithMultipleCatchClauses()
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