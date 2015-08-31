using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NonEncapsulatedOrMutableField;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NonEncapsulatedOrMutableFieldTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NonEncapsulatedOrMutableFieldAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new NonEncapsulatedOrMutableFieldCodeFix();

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithInternalField_AndInlineInitialization()
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

            VerifyDiagnostic(original, string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithInlineInitialization()
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

            VerifyDiagnostic(original, string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithPublicField()
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

            VerifyDiagnostic(original, string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithPrivateField()
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
        public void NonEncapsulatedOrMutableField_WithProtectedInternalField()
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

            VerifyDiagnostic(original, string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithConstField()
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
        public void NonEncapsulatedOrMutableField_WithReadOnlyField()
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
        public void NonEncapsulatedOrMutableField_WithMultipleDeclarators()
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

            VerifyDiagnostic(original,
                string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"),
                string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "y"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithMultipleDeclaratorsAndInlineInitialization()
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

            VerifyDiagnostic(original,
                string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"),
                string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "y"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithAttribute()
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

            VerifyDiagnostic(original,
                string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "x"),
                string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "y"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithVerbatimIdentifier()
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

            VerifyDiagnostic(original, string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "@class"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NonEncapsulatedOrMutableField_WithEscapedIdentifier()
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

            VerifyDiagnostic(original, string.Format(NonEncapsulatedOrMutableFieldAnalyzer.Rule.MessageFormat.ToString(), "\\u0061ss"));
            VerifyFix(original, result);
        }
    }
}