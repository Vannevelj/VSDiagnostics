using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.FlagsEnumValuesAreNotPowersOfTwo;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class FlagsEnumValuesAreNotPowersOfTwoTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new FlagsEnumValuesAreNotPowersOfTwoAnalyzer();

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = 4,
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8,
    }
}";

            VerifyDiagnostic(original);
        }
    }
}