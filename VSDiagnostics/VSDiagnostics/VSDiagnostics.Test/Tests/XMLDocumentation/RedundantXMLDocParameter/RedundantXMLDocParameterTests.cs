using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.XMLDocumentation.RedundantXMLDocParameter;

namespace VSDiagnostics.Test.Tests.XMLDocumentation.RedundantXMLDocParameter
{
    [TestClass]
    public class RedundantXmlDocParameterTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RedundantXmlDocParameterAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new RedundantXmlDocParameterCodeFix();

        [TestMethod]
        public void RedundantXmlDocParameter_DoesNotFireForValidNodes()
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
        public void RedundantXmlDocParameter_FiresForNodeWithNonexistentParameter()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myParam"">A nonexistent parameter</param>
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
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocParameter_RemovesSingleNodeWithoutAffectingOthers()
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
        /// <param name=""myParam"">A nonexistent parameter</param>
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
        /// <param name=""myInt"">A nonexistent parameter</param>
        public void Fizz(int myInt)
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocParameter_RemovesWhenOnlyParamNodeExists()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <param name=""myParam"">A nonexistent parameter</param>
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
        
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocParameter_OnlyRemovesParamClause()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""myParam"">A nonexistent parameter</param>
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
        [System.Obsolete("""")]
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocParameter_OnlyRemovesParamClause_DoesNotRemoveOtherClauses()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// 
        /// </summary> <param name=""myParam"">A nonexistent parameter</param>
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
        [System.Obsolete("""")]
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantXmlDocParameter_OnlyRemovesExplicitReturnsNode()
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
        public void RedundantXmlDocParameter_OnlyRemovesParamClause_DocumentAllTextBeforeNodeRemoved()
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
        /// <param name=""myParam"">A nonexistent parameter</param>
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
        /// text isn't usually outside XML nodes...
        [System.Obsolete("""")]
        public void Fizz()
        {
        }
    }
}";

            VerifyDiagnostic(original, RedundantXmlDocParameterAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}