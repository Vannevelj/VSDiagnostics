using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.XMLDocumentation.MisssingXmlDocSummary;

namespace VSDiagnostics.Test.Tests.XMLDocumentation.MissingXmlDocSummary
{
    [TestClass]
    public class MissingXmlDocSummaryTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new MissingXmlDocSummaryAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new MissingXmlDocSummaryCodeFix();

        [TestMethod]
        public void MissingXmlDocSummary_DoesNotFireForMethodsWithSummaryInDoc()
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
        public void MissingXmlDocSummary_AddsSummaryNodeWithoutAffectingOthers()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <param name=""myInt"">An unnecessary parameter</param>
        /// <param name=""myString"">An unnecessary parameter</param>
        /// <returns></returns>
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
        /// {summaryInfo}
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

            VerifyDiagnostic(original, MissingXmlDocSummaryAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingXmlDocSummary_DoesNotFireWhenNoDocExists()
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
        public void MissingXmlDocSummary_SavesAllTextBeforeNodeAdded()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
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

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// {summaryInfo}
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

            VerifyDiagnostic(original, MissingXmlDocSummaryAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}