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
        protected override CodeFixProvider CodeFixProvider => new NonEncapsulatedOrMutableFieldCodeFix();

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
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 22)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithInlineInitialization_InvokesWarning()
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
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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
        protected internal int X { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 32)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithMultipleDeclarators_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int x, y;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int X { get; set; }

        internal int Y { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 22)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "y"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 25)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithMultipleDeclaratorsAndInlineInitialization_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int x = 5, y;
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

        internal int Y { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 22)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "y"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 29)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        [MyAttribute]
        public int x, y;
    }

    class MyAttribute : Attribute
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
        [MyAttribute]
        public int X { get; set; }

        [MyAttribute]
        public int Y { get; set; }
    }

    class MyAttribute : Attribute
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "x"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 20)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "y"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 23)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithVerbatimIdentifier_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int @class;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int @class { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "@class"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableFieldAnalyzer_WithEscapedIdentifier_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int \u0061ss;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int \u0061ss { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NonEncapsulatedOrMutableFieldAnalyzer.DiagnosticId,
                Message = string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Message, "\\u0061ss"),
                Severity = NonEncapsulatedOrMutableFieldAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }
    }
}