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
        int Method()
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
        int Method() => 5;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = SimplifyExpressionBodiedMemberAnalyzer.DiagnosticId,
                Message = SimplifyExpressionBodiedMemberAnalyzer.Message,
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, expected);
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
                Message = SimplifyExpressionBodiedMemberAnalyzer.Message,
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, expected);
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
                Message = SimplifyExpressionBodiedMemberAnalyzer.Message,
                Severity = SimplifyExpressionBodiedMemberAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, expected);
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
        int Method()
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
        void Method()
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
        int Method()
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
    }
}