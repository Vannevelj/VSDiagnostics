using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsDefaultOptions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionalOperatorReturnsDefaultOptionsAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionalOperatorReturnsDefaultOptionsAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionalOperatorReturnsDefaultOptionsCodeFix();

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithOnlyLiterals_InvokesWarning()
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
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge ? true : false;
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
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.Id,
                Message = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.MessageFormat.ToString(),
                Severity = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithTrueConditionAsLiteral_DoesNotInvokeWarning()
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
            bool somethingElse = false;
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge ? true : somethingElse;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithFalseConditionAsLiteral_DoesNotInvokeWarning()
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
            bool somethingElse = true;
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge ? somethingElse : false;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithOnlyLiterals_AsReturnStatement_InvokesWarning()
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
            int legalAge = 18;
            int myAge = 22;
            return myAge >= legalAge ? true : false;
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
            int legalAge = 18;
            int myAge = 22;
            return myAge >= legalAge;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.Id,
                Message = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.MessageFormat.ToString(),
                Severity = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithStrings_DoesNotInvokeWarning()
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
            int legalAge = 18;
            int myAge = 22;
            string ageGroup = myAge >= legalAge ? ""Adult"" : ""Child"";
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithMoreComplicatedCondition_InvokesWarning()
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
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = (myAge >= legalAge && myAge < 100) ? true : false;
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
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = (myAge >= legalAge && myAge < 100);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.Id,
                Message = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.MessageFormat.ToString(),
                Severity = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 29)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithInvertedBooleanLiterals_DoesNotInvokeWarning()
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
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge ? false : true;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptionsAnalyzer_WithLiteralsAsString_DoesNotInvokeWarning()
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
            int legalAge = 18;
            int myAge = 22;
            string canDrink = myAge >= legalAge ? ""true"" : ""false"";
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}