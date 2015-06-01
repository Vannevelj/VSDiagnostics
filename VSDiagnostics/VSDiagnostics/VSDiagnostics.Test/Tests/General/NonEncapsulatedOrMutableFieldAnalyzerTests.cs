using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NonEncapsulatedOrMutableFieldAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NonEncapsulatedOrMutableFieldAnalyzer();
        protected override CodeFixProvider CodeFixProvider { get; }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithInternalField_AndInlineInitialization_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int x = 5;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int X { get; set; } = 5;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = NonEncapsulatedOrMutableFieldAnalyzer.Message,
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 0, 0)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithPublicField_AndInlineInitialization_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int x = 5;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int X { get; set; } = 5;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = NonEncapsulatedOrMutableFieldAnalyzer.Message,
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 0, 0)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithPublicField_InvokesWarning()
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
        public int X { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = NonEncapsulatedOrMutableFieldAnalyzer.Message,
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 0, 0)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithPrivateField_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int x;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithProtectedInternalField_InvokesWarning()
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
        protected internal int X { get; set; };
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = NonEncapsulatedOrMutableFieldAnalyzer.Message,
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 0, 0)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            //VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithConstField_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal const int x;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithReadOnlyField_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal readonly int x;
    }
}";

            VerifyDiagnostic(original);
        }
    }
}