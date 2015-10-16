using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.GotoDetection;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class GotoDetectionTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new GotoDetectionAnalyzer();

        [TestMethod]
        public void GotoDetection_GotoCaseStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var cost = 2;
            switch (cost)
            {
                case 1:
                    cost = 4;
                    break;
                case 2:
                    goto case 1;
                default:
                    cost = -1;
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, GotoDetectionAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void GotoDetection_GotoDefaultStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var cost = 1;
            switch (cost)
            {
                case 1:
                    goto default;
                default:
                    cost = 4;
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, GotoDetectionAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void GotoDetection_GotoLabel()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            End:

            var i = 0;
            if (i == 0) { goto End; }
        }
    }
}";

            VerifyDiagnostic(original, GotoDetectionAnalyzer.Rule.MessageFormat.ToString());
        }
    }
}