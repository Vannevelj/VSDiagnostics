using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.EqualsAndGetHashcodeNotImplementedTogether;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class EqualsAndGetHashcodeNotImplementedTogetherTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EqualsAndGetHashcodeNotImplementedTogetherAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new EqualsAndGetHashcodeNotImplementedTogetherCodeFix();

        [TestMethod]
        public void EqualsAndGetHashcodeNotImplemented_BothImplemented_NoWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EqualsAndGetHashcodeNotImplemented_EqualsImplemented()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, EqualsAndGetHashcodeNotImplementedTogetherAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void EqualsAndGetHashcodeNotImplemented_GetHashcodeImplemented()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, EqualsAndGetHashcodeNotImplementedTogetherAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EqualsAndGetHashcodeNotImplemented_NeitherImplemented_NoWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EqualsAndGetHashcodeNotImplemented_NonOverridingEqualsImplemented_NoWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EqualsAndGetHashcodeNotImplemented_NonOverridingGetHashcodeImplemented_NoWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}