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
        public void AsToCast_PredefinedType()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var ch = 'r';
            object o = ch;
            var i = o as int?;
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
            object o = ch;
            var i = (int?)o;
        }
    }
}";

            VerifyDiagnostic(original, AsToCastAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsToCast_MethodCall()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Main()
        {
            bool? b = GetBoxedType() as bool?;
        }

        object GetBoxedType()
        {
            return true;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Main()
        {
            bool? b = (bool?)GetBoxedType();
        }

        object GetBoxedType()
        {
            return true;
        }
    }
}";

            VerifyDiagnostic(original, AsToCastAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsToCast_CustomType()
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
            var i = variable as Program;
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
            var i = (Program)variable;
        }
    }
}";

            VerifyDiagnostic(original, AsToCastAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsToCast_OnlyFormatSpecificNode()
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
            var i = variable as Program;

            var j = (Program) variable;    // make sure this isn't formatted to '(Program)variable'
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
            var i = (Program)variable;

            var j = (Program) variable;    // make sure this isn't formatted to '(Program)variable'
        }
    }
}";

            VerifyDiagnostic(original, AsToCastAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}