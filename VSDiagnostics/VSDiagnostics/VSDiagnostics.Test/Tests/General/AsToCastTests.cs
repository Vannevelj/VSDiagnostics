using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.AsToCast;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class AsToCastTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AsToCastAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new AsToCastCodeFix();

        [TestMethod]
        public void AsToCast_PredefinedType_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var ch = 'r';
            var i = ch as int;
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
            var ch = 'r';
            var i = (int)ch;
        }
    }
}";

            VerifyDiagnostic(original, AsToCastAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsToCast_CustomType_InvokesWarning()
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
            var i = ch as Program;
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
            var i = (Program)ch;
        }
    }
}";

            VerifyDiagnostic(original, AsToCastAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}