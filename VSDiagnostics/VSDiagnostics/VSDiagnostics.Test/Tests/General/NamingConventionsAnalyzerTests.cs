using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NamingConventions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NamingConventionsAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NamingConventionsAnalyzer();
        protected override CodeFixProvider CodeFixProvider { get; }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPrivateField_AndCapitalStart_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int X;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "field", "X", "_x"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPrivateField_AndCapitalStartWithUnderscore_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _X;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "field", "_X", "_x"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPublicField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProtectedField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInternalField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProtectedInternalField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected internal int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected internal int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProperty_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int x { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int X { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "property", "x", "X"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithMethod_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void method()
        {
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "method", "method", "Method"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithClass_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class myClass
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "class", "myClass", "MyClass"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithoutPrefix_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface something
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "interface", "something", "ISomething"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithPrefix_AndLowerSecondLetter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface Isomething
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "interface", "Isomething", "ISomething"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithlowerPrefix_AndCapitalSecondLetter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface iSomething
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "interface", "iSomething", "ISomething"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithLocalVariable_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        var MyVar = 5;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        var myVar = 5;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "local", "MyVar", "myVar"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithParameter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(string Param)
        {
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(string param)
        {
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.DiagnosticId,
                Message = string.Format(NamingConventionsAnalyzer.Message, "parameter", "Param", "param"),
                Severity = NamingConventionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }
    }
}