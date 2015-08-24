using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionIsAlwaysFalse;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionIsAlwaysFalseTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionIsAlwaysFalseAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionIsAlwaysFalseCodeFix();

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (false)
            {
                var b = true;
                b = false;
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
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (false)
                b = false;

            if (b) { } // prevent variable is never used warning for fix
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
            var b = true;

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConditionContainsTrueLiteral()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (b && false)
                b = false;
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}