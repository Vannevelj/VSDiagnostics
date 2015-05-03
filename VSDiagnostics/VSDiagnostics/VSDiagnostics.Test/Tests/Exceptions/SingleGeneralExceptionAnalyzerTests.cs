using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.Exceptions.SingleGeneralExceptionAnalyzer;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class SingleGeneralExceptionAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithSingleGeneralException_InvokesWarning()
        {
        }

        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithoutNamedCatchClauses_DoesNotInvokeWarning()
        {
        }

        [TestMethod]
        public void SingleGeneralExceptionAnalyzer_WithMultipleCatchClauses_DoesNotInvokeWarning()
        {
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SingleGeneralExceptionAnalyzer();
        }
    }
}