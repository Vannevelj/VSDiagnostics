﻿using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            VerifyDiagnostic(test, CatchNullReferenceExceptionAnalyzer.Rule.MessageFormat.ToString());
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