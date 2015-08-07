using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.CastToAs;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class CastToAsAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CastToAsAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new CastToAsCodeFix();

        [TestMethod]
        public void CastToAsAnalyzer_PredefinedType_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var ch = 'r';
            var i = (int) ch;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CastToAsAnalyzer_CustomType_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    interface P
    {
    }

    class Program : P
    {
    }

    class MyClass
    {
        void Method()
        {
            P variable = new Program();
            var i = (Program) ch;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    interface P
    {
    }

    class Program : P
    {
    }

    class MyClass
    {
        void Method()
        {
            P variable = new Program();
            var i = ch as Program;
        }
    }
}";

            VerifyDiagnostic(original, CastToAsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}