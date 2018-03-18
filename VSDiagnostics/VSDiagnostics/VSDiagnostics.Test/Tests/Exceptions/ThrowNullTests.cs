using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.ThrowNull;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class ThrowNullTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ThrowNullAnalyzer();

        [TestMethod]
        public void ThrowNull_ThrowsNull()
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
                throw null;
            }
        }
    }";

            VerifyDiagnostic(original, "Throwing null will always result in a runtime exception");
        }

        [TestMethod]
        public void ThrowNull_DoesNotThrowNull()
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
    }
}
