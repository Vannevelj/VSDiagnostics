using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.PrivateSetAutoPropertyCanBeReadOnlyAutoProperty;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyTests : CodeFixVerifier
    {
        [TestMethod]
        public void PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyAnalyzer_WithConstructorAccessOnly_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int SomeProperty {get; private set;}
   
        MyClass()
        {
            this.SomeProperty = 42;
        }

        void SomeMethod()
        {
            bool isAwesome = true;
            if (isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = PrivateSetAutoPropertyCanBeReadOnlyAutoPropertyAnalyzer.DiagnosticId,
                Message = PrivateSetAutoPropertyCanBeReadOnlyAutoPropertyAnalyzer.Message,
                Severity = PrivateSetAutoPropertyCanBeReadOnlyAutoPropertyAnalyzer.Severity,

                Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 8, 31)
                        }
            };

            
            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //todo: create fix
            //VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyAnalyzer_WithMethodAccess_DoesNotInvokesWarning()
        {

        }
    }
}
