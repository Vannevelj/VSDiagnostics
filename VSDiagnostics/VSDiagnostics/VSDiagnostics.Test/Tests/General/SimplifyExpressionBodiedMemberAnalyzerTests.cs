using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SimplifyExpressionBodiedMemberAnalyzerTests : CodeFixVerifier
    {
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

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, expected);
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

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, expected);
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

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, expected);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMemberAnalyzer_WithMethodAndTrailingComments_InvokesWarning()
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

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod() => 5; /* comments */
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

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, expected);
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
            VerifyCSharpDiagnostic(original);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SimplifyExpressionBodiedMemberAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SimplifyExpressionBodiedMemberCodeFix();
        }
    }
}