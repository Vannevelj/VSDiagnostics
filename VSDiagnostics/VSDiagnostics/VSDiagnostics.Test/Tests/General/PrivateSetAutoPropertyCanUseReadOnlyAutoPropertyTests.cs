using System;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.PrivateSetAutoPropertyCanBeReadOnlyAutoProperty;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider
        {
            get
            {
                //todo: create quick fix
                throw new NotImplementedException();
            }
        }

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new PrivateSetAutoPropertyCanBeReadOnlyAutoPropertyAnalyzer();

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
            Console.WriteLine(""awesome"");
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
                            new DiagnosticResultLocation("Test0.cs", 9, 32)
                        }
            };


            VerifyDiagnostic(original, expectedDiagnostic);
            //todo: create fix
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyAnalyzer_WithOnlyGetterAccess_InvokesWarning()
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
            Console.WriteLine(SomeProperty);
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
                            new DiagnosticResultLocation("Test0.cs", 9, 32)
            }
            };


            VerifyDiagnostic(original, expectedDiagnostic);
            //todo: create fix
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyAnalyzer_WithMethodAccess_DoesNotInvokesWarning()
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
            this.SomeProperty = 10;
        }
    }
}";

            VerifyDiagnostic(original);
            //todo: create quick fix
        }

        [TestMethod]
        public void PrivateSetAutoPropertyCanUseReadOnlyAutoPropertyAnalyzer_WithReadOnlyProperty_DoesNotInvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int SomeProperty { get; } = 42;
        int SomeOtherProperty { get; set; };
   
        MyClass()
        {
            this.SomeOtherProperty = 27;
        }

        void SomeMethod()
        {
            Console.WriteLine(this.SomeOtherProperty);
        }
    }
}";

            VerifyDiagnostic(original);
            //todo: create quickfix
        }
    }
}
