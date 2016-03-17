using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.CatchNullReferenceException;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class CatchNullReferenceExceptionTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CatchingNullReferenceExceptionAnalyzer();

        [TestMethod]
        public void CatchNullReferenceException_WithNullReferenceCatchClause()
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

            VerifyDiagnostic(test, CatchingNullReferenceExceptionAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void CatchNullReferenceException_WithoutNullReferenceCatchClause()
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
        public void CatchNullReferenceException_WithEmptyCatchClause()
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