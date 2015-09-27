using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.XMLDocComments.RedundantXMLDocReturn;

namespace VSDiagnostics.Test.Tests.XMLDocComments.RedundantXMLDocReturn
{
    [TestClass]
    public class RedundantXmlDocReturnTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RedundantXmlDocReturnAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new RedundantXmlDocReturnCodeFix();

        [TestMethod]
        public void RedundantXmlDocReturn_DoesNotFireForNonVoidMethod()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Fizz()
        {
            return 3;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantXmlDocReturn_FiresForNonVoidMethod()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Fizz()
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
        /// 
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocReturnAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocReturn_FiresForNonVoidMethod_OnlyRemovesReturnClause()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [System.Obsolete("""")]
        public void Fizz()
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
        /// 
        [System.Obsolete("""")]
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocReturnAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocReturn_FiresForNonVoidMethod_OnlyRemovesExplicitReturnsNode()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// <returns></returns>
        /// </summary>
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}