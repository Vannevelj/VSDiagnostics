using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class CompareBooleanToTrueLiteralAnalyzerTests : CodeFixVerifier
    {
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
            if(isAwesome == true)
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
            if(isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = CompareBooleanToTrueLiteralAnalyzer.DiagnosticId,
                Message = CompareBooleanToTrueLiteralAnalyzer.Message,
                Severity = CompareBooleanToTrueLiteralAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteralAnalyzer_WithSimpleEqualsTrueLiteralComparison_InvokesWarning()
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
            if(isAwesome.Equals(true))
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
            if(isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = CompareBooleanToTrueLiteralAnalyzer.DiagnosticId,
                Message = CompareBooleanToTrueLiteralAnalyzer.Message,
                Severity = CompareBooleanToTrueLiteralAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, result);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = CompareBooleanToTrueLiteralAnalyzer.DiagnosticId,
                Message = CompareBooleanToTrueLiteralAnalyzer.Message,
                Severity = CompareBooleanToTrueLiteralAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, result);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = CompareBooleanToTrueLiteralAnalyzer.DiagnosticId,
                Message = CompareBooleanToTrueLiteralAnalyzer.Message,
                Severity = CompareBooleanToTrueLiteralAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, result);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CompareBooleanToTrueLiteralAnalyzer();
        }
    }
}