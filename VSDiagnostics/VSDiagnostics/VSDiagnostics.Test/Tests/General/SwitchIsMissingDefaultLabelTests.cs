using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SwitchIsMissingDefaultLabel;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SwitchIsMissingDefaultLabelTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SwitchIsMissingDefaultLabelAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new SwitchIsMissingDefaultLabelCodeFix();

        [TestMethod]
        public void SwitchIsMissingDefaultLabel_MissingDefaultStatement_SwitchOnEnum()
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

            var result = @"
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
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchIsMissingDefaultLabelAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchIsMissingDefaultLabel_MissingDefaultStatement_SwitchOnString()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = ""test"";
            switch (e)
            {
                case ""test"":
                case ""test1"":
                    break;
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
            var e = ""test"";
            switch (e)
            {
                case ""test"":
                case ""test1"":
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchIsMissingDefaultLabelAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchIsMissingDefaultLabel_HasDefaultStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = ""test"";
            switch (e)
            {
                case ""test"":
                case ""test1"":
                default:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SwitchIsMissingDefaultLabel_MissingDefaultStatement_ParenthesizedStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var x = 5;
            switch ((x))
            {
                case 5: 
                case 6:
                    break;
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
            var x = 5;
            switch ((x))
            {
                case 5: 
                case 6:
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchIsMissingDefaultLabelAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}