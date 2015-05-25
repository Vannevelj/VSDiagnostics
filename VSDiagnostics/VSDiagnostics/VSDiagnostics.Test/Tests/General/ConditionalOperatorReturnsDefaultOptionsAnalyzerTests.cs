using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsDefaultOptions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionalOperatorReturnsDefaultOptionsAnalyzerTests : CodeFixVerifier
    {
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
                Id = ConditionalOperatorReturnsDefaultOptionsAnalyzer.DiagnosticId,
                Message = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Message,
                Severity = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, result);
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

            VerifyCSharpDiagnostic(original);
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

            VerifyCSharpDiagnostic(original);
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
                Id = ConditionalOperatorReturnsDefaultOptionsAnalyzer.DiagnosticId,
                Message = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Message,
                Severity = ConditionalOperatorReturnsDefaultOptionsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            //VerifyCSharpFix(original, result);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConditionalOperatorReturnsDefaultOptionsAnalyzer();
        }
    }
}