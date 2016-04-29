using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Structs.StructWithoutElementaryMethodsOverridden;

namespace VSDiagnostics.Test.Tests.Structs
{
    [TestClass]
    public class StructWithoutElementaryMethodsOverriddenTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new StructWithoutElementaryMethodsOverriddenAnalyzer();

        [TestMethod]
        public void StructWithoutElementaryMethodsOverridden_NoMethodsImplemented()
        {
            var original = @"
struct X
{
}";

            VerifyDiagnostic(original, StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StructWithoutElementaryMethodsOverridden_EqualsImplemented()
        {
            var original = @"
struct X
{
    public override bool Equals(object obj)
    {
        return false;
    }
}";

            VerifyDiagnostic(original, StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StructWithoutElementaryMethodsOverridden_GetHashCodeImplemented()
        {
            var original = @"
struct X
{
    public override int GetHashCode()
    {
        return 0;
    }
}";

            VerifyDiagnostic(original, StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StructWithoutElementaryMethodsOverridden_ToStringImplemented()
        {
            var original = @"
struct X
{
    public override string ToString()
    {
        return string.Empty;
    }
}";

            VerifyDiagnostic(original, StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StructWithoutElementaryMethodsOverridden_EqualsAndGetHashCodeImplemented()
        {
            var original = @"
struct X
{
    public override bool Equals(object obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}";

            VerifyDiagnostic(original, StructWithoutElementaryMethodsOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StructWithoutElementaryMethodsOverridden_AllImplemented()
        {
            var original = @"
struct X
{
    public override bool Equals(object obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}";

            VerifyDiagnostic(original);
        }
    }
}