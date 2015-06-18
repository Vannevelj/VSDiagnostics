using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SimplifyExpressionBodiedMemberAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SimplifyExpressionBodiedMemberAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new SimplifyExpressionBodiedMemberCodeFix();

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithSimpleReturnMethod_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            return 5;
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod() => 5;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId,
                Message = string.Format(SimplifyExpressionBodiedMemberAnalyzer.Message, "Method", "MyMethod"),
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithSimpleGetterOnlyProperty_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty { get { return ""myString""; } }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty => ""myString"";
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId,
                Message = string.Format(SimplifyExpressionBodiedMemberAnalyzer.Message, "Property", "MyProperty"),
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 35)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMultiLineGetterOnlyProperty_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty 
        { 
            get 
            {
                return ""myString"";
            } 
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty => ""myString"";
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId,
                Message = string.Format(SimplifyExpressionBodiedMemberAnalyzer.Message, "Property", "MyProperty"),
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 17)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMultiStatementGetterOnlyProperty_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty 
        { 
            get 
            {
                var newValue = ""myString"" + "".AnotherString"";
                return newValue; 
            } 
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMultiStatementMethod_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            var result = 5 * 5;
            return result;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMethodWithVoidReturn_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void MyMethod()
        {
            var result = 5 * 5;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMethodWithoutReturn_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            while(true)
            {
                Console.WriteLine(""This is a weird feature"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMethodWithExpressionBody_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod() => 5;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithPropertyWithExpressionBody_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty => 5;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMethodWithUnreachableCode_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            return 5;
            int x = 6;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMethodAndTrailingComments_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            return 5; /* comments */
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithPropertyWithGetAndSet_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { get; set; }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMultipleProperties_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { get { return 5; } }
        int MyProperty2 { get { return 6; } }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty => 5;

        int MyProperty2 => 6;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId,
                Message = string.Format(SimplifyExpressionBodiedMemberAnalyzer.Message, "Property", "MyProperty"),
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 32)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId,
                Message = string.Format(SimplifyExpressionBodiedMemberAnalyzer.Message, "Property", "MyProperty2"),
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 33)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithTrivia_AsProperty_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { /* test */ get { return 5; } /* more test */ }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithTrivia_AsMethod_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod() 
        { 
            /* test */ 
            return 5;
            /* more test */
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithTrailingPropertyTrivia_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { get { return 5; }} // lala
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithAttribute_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { [MyAttribute] get { return 5; }} 
    }

    [AttributeUsage(AttributeTargets.All)]
    class MyAttribute : Attribute
    {   
    }
}";

            VerifyDiagnostic(original);
        }
    }
}