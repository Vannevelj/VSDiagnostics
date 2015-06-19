using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class CompareBooleanToTrueLiteralAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CompareBooleanToTrueLiteralAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new CompareBooleanToTrueLiteralCodeFix();

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithSimpleTrueLiteralComparison_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
            if (isAwesome == true)
            {
                Console.WriteLine(""awesome"");
            }
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
            bool isAwesome = true;
            if (isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithSimpleTrueLiteralComparisonAsReturnValue_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome == true;
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
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithComplicatedTrueLiteralComparison_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"" == true).ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"").ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithUnrelatedTrueLiteral_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithSimplifiedExpression_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_ComparedToBooleanAsString_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            if (""someString"" == ""true"")
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithTrueLiteralAsLefthandValue_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return true == isAwesome;
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithOtherOperator_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool condition = false;
            if (condition != true)
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}