using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsDefaultOptions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionalOperatorReturnsDefaultOptionsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionalOperatorReturnsDefaultOptionsAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionalOperatorReturnsDefaultOptionsCodeFix();

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptions_WithOnlyLiterals()
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

            VerifyDiagnostic(original, ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptions_WithTrueConditionAsLiteral()
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
        public void ConditionalOperatorReturnsDefaultOptions_WithFalseConditionAsLiteral()
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
        public void ConditionalOperatorReturnsDefaultOptions_WithOnlyLiterals_AsReturnStatement()
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

            VerifyDiagnostic(original, ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptions_WithStrings()
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
        public void ConditionalOperatorReturnsDefaultOptions_WithMoreComplicatedCondition()
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

            VerifyDiagnostic(original, ConditionalOperatorReturnsDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsDefaultOptions_WithInvertedBooleanLiterals()
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
        public void ConditionalOperatorReturnsDefaultOptions_WithLiteralsAsString()
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