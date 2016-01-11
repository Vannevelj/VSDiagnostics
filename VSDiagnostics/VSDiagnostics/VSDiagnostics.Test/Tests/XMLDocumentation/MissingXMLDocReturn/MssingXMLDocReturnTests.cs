using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.XMLDocumentation.MisssingXmlDocReturn;

namespace VSDiagnostics.Test.Tests.XMLDocumentation.MissingXmlDocReturn
{
    [TestClass]
    public class MissingXmlDocReturnTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new MissingXmlDocReturnAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new MissingXmlDocReturnCodeFix();

        [TestMethod]
        public void MissingXmlDocReturn_DoesNotFireForMethodsWithReturnInDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myInt"">An unnecessary parameter</param>
        /// <param name=""myString"">An unnecessary parameter</param>
        /// <returns></returns>
        public int Fizz(int myInt, string myString)
        {
            return 3;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingXmlDocReturn_DoesNotFireForVoidMethod()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingXmlDocReturn_AddsReturnNodeWithoutAffectingOthers()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myInt"">An unnecessary parameter</param>
        /// <param name=""myString"">An unnecessary parameter</param>
        public int Fizz(int myInt, string myString)
        {
            return 0;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myInt"">An unnecessary parameter</param>
        /// <param name=""myString"">An unnecessary parameter</param>
        /// <returns></returns>
        public int Fizz(int myInt, string myString)
        {
            return 0;
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocReturnAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingXmlDocReturn_DoesNotFireWhenNoDocExists()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int Fizz(int myParam)
        {
            return 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingXmlDocReturn_SummaryNodeDoesNotExist()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <param name=""myParam""></param>
        public int Fizz(int myParam)
        {
            return 0;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <param name=""myParam""></param>
        /// <returns></returns>
        public int Fizz(int myParam)
        {
            return 0;
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocReturnAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingXmlDocReturn_AddsNodeWhenNodeExistsInIncorrectPlace()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// <returns></returns>
        /// </summary>
        public int Fizz()
        {
            return 0;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// <returns></returns>
        /// </summary>
        /// <returns></returns>
        public int Fizz()
        {
            return 0;
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocReturnAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingXmlDocReturn_SavesAllTextBeforeNodeAdded()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myInt""></param>
        /// text isn't usually outside XML nodes...
        /// more text outside a node...
        [System.Obsolete("""")]
        public int Fizz(int myInt)
        {
            return 0;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myInt""></param>
        /// text isn't usually outside XML nodes...
        /// more text outside a node...
        /// <returns></returns>
        [System.Obsolete("""")]
        public int Fizz(int myInt)
        {
            return 0;
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocReturnAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}