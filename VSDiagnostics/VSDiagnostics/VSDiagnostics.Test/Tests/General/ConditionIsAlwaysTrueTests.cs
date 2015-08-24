using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionIsAlwaysTrue;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionIsAlwaysTrueTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionIsAlwaysTrueAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionIsAlwaysTrueCodeFix();

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (true)
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
            var b = true;
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (true)
                b = false;
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
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConditionContainsTrueLiteral()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (b && true)
                b = false;
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}