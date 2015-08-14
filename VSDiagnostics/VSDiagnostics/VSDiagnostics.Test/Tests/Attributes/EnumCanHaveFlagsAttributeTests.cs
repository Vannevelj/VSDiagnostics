using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class EnumCanHaveFlagsAttributeTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EnumCanHaveFlagsAttributeAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new EnumCanHaveFlagsAttributeCodeFix();

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute_OnlyAddsFlagsAttribute()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [Obsolete(""I'm obsolete"")]
    enum Foo
    {
        Goo = 0,
        Hoo,
        Joo,
        Koo,
        Loo
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [Obsolete(""I'm obsolete"")]
    [Flags]
    enum Foo
    {
        Goo = 0,
        Hoo,
        Joo,
        Koo,
        Loo
    }
}";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}