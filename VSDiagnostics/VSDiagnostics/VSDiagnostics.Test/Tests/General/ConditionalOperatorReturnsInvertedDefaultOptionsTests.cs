using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionalOperatorReturnsInvertedDefaultOptions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionalOperatorReturnsInvertedDefaultOptionsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionalOperatorReturnsInvertedDefaultOptionsCodeFix();

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithOnlyLiterals()
        {
            var original = @"
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

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = !(myAge >= legalAge);
        }
    }
}";

            VerifyDiagnostic(original, ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_AbsoluteBasic()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var foo = true;
            var bar = foo ? false : true;
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
            var foo = true;
            var bar = !foo;
        }
    }
}";

            VerifyDiagnostic(original, ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithTrueConditionAsLiteral()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            bool somethingElse = true;
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge ? false : somethingElse;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithFalseConditionAsLiteral()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            bool somethingElse = false;
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = myAge >= legalAge ? somethingElse : true;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithOnlyLiterals_AsReturnStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            int legalAge = 18;
            int myAge = 22;
            return myAge >= legalAge ? false : true;
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
            int legalAge = 18;
            int myAge = 22;
            return !(myAge >= legalAge);
        }
    }
}";

            VerifyDiagnostic(original, ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithStrings()
        {
            var original = @"
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
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithMoreComplicatedCondition()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = (myAge >= legalAge && myAge < 100) ? false : true;
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
            int legalAge = 18;
            int myAge = 22;
            bool canDrink = !(myAge >= legalAge && myAge < 100);
        }
    }
}";

            VerifyDiagnostic(original, ConditionalOperatorReturnsInvertedDefaultOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithBooleanLiterals()
        {
            var original = @"
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionalOperatorReturnsInvertedDefaultOptions_WithLiteralsAsString()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            int legalAge = 18;
            int myAge = 22;
            string canDrink = myAge >= legalAge ? ""false"" : ""true"";
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}