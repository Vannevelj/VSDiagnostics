using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SwitchDoesNotHandleAllEnumOptions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SwitchDoesNotHandleAllEnumOptionsTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SwitchDoesNotHandleAllEnumOptionsAnalyzer();

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_AllEnumStatements()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                case MyEnum.FizzBuzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_CaseStatementsNotEnum()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            switch (""test"")
            {
                case ""Fizz"":
                case ""Buzz"":
                case ""FizzBuzz"":
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_CaseHasDefaultStatement_MissingEnumStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                default:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement_MultipleSections()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                    break;
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_UsingStaticEnum_MissingEnumStatements()
        {
            var original = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = FileOptions.DeleteOnClose;
            switch (e)
            {
                case Asynchronous:
                case DeleteOnClose:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
        }
    }
}