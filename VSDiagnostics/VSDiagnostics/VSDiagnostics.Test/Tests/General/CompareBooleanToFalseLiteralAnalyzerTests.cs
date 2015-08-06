using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.CompareBooleanToFalseLiteral;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class CompareBooleanToFalseLiteralAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CompareBooleanToFalseLiteralAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new CompareBooleanToFalseLiteralCodeFix();

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithSimpleFalseLiteralComparison_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = false;
            if (isAwesome == false)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = false;
            if (!isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithSimpleFalseLiteralComparisonAsReturnValue_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return isAwesome == false;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return !isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithComplicatedFalseLiteralComparison_FirstComparisonIsEquals_InvokesWarning()
        {
            var original = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"" == false).ToArray();
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
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"").ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithComplicatedFalseLiteralComparison_FirstComparisonIsNotEquals_InvokesWarning()
        {
            var original = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"" == false).ToArray();
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

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithUnrelatedFalseLiteral_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = False;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithSimplifiedExpression_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return !isAwesome;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_ComparedToBooleanAsString_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            if (""someString"" == ""false"")
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithFalseLiteralAsLefthandValue_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = False;
            return false == isAwesome;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = False;
            return !isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithSimpleFalseLiteralComparison_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = false;
            if (isAwesome != false)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = false;
            if (isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithSimpleFalseLiteralComparisonAsReturnValue_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return isAwesome != false;
        }
    }
}";

            var result = @"
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

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithComplicatedFalseLiteralComparison_NotEqualsOperator_FirstComparisonIsEquals_InvokesWarning()
        {
            var original = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"" != false).ToArray();
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

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithComplicatedFalseLiteralComparison_NotEqualsOperator_FirstComparisonIsNotEquals_InvokesWarning()
        {
            var original = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"" == false).ToArray();
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

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_ComparedToBooleanAsString_NotEqualsOperator_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            if (""someString"" != ""false"")
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithFalseLiteralAsLefthandValue_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = False;
            return false != isAwesome;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = False;
            return isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithNullableBool_DoesNotInvokeWarning()
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
            bool? b = null;
            if(b == false)
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_WithNullableBool_AsMethodInvocation_DoesNotInvokeWarning()
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
            if(GetBoolean() == false)
            {

            }
        }

        bool? GetBoolean()
        {
            return false;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToFalseLiteralAnalyzer_AsMethodInvocation_InvokesWarning()
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
            if (GetBoolean() == false)
            {

            }
        }

        bool GetBoolean()
        {
            return false;
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
            if (!GetBoolean())
            {

            }
        }

        bool GetBoolean()
        {
            return false;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToFalseLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}