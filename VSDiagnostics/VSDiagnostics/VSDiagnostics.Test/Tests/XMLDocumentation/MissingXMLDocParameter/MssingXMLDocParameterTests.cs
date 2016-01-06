using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.XMLDocumentation.MissingXMLDocParameter;

namespace VSDiagnostics.Test.Tests.XMLDocumentation.MissingXMLDocParameter
{
    [TestClass]
    public class MissingXmlDocParameterTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new MissingXmlDocParameterAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new MissingXmlDocParameterCodeFix();

        [TestMethod]
        public void MissingXmlDocParameter_DoesNotFireForMethodsWithAllParamsInDoc()
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
            return 3;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingXmlDocParameter_FiresForNodeWithUndocumentedParameter()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        public void Fizz(int myParam)
        {
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
        /// <param name=""myParam""></param>
        public void Fizz(int myParam)
        {
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingXmlDocParameter_AddsSingleNodeWithoutAffectingOthers()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myInt"">A nonexistent parameter</param>
        public void Fizz(int myInt, string myString)
        {
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
        /// <param name=""myInt"">A nonexistent parameter</param>
        /// <param name=""myString""></param>
        public void Fizz(int myInt, string myString)
        {
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingXmlDocParameter_DoesNotFireWhenNoDocExists()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingXmlDocParameter_OnlyRemovesExplicitReturnsNode()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// <param name=""myParam"">A nonexistent parameter</param>
        /// </summary>
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingXmlDocParameter_OnlyRemovesParamClause_DocumentAllTextBeforeNodeRemoved()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// text isn't usually outside XML nodes...
        [System.Obsolete("""")]
        public void Fizz(int myInt)
        {
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
        [System.Obsolete("""")]
        public void Fizz(int myInt)
        {
        }
    }
}";

            VerifyDiagnostic(original, MissingXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}