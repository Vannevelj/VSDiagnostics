using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class EnumCanHaveFlagsAttributeCSharpTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EnumCanHaveFlagsAttributeAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new EnumCanHaveFlagsAttributeCodeFix();

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute()
        {
            var original = 
@"namespace ConsoleApplication1
{
    enum Foo
    {
    }
}";

            var result = 
@"using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
    }
}";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute_DoesNotAddDuplicateUsingSystem()
        {
            var original =
@"using System;

namespace ConsoleApplication1
{
    enum Foo
    {
    }
}";

            var result =
@"using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
    }
}";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute_OnlyAddsFlagsAttribute()
        {
            var original =
@"using System;

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

            var result =
@"using System;

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

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_EnumHasXmlDocComment_OnlyAddsFlagsAttribute()
        {
            var original =
@"namespace ConsoleApplication1
{
    /// <summary>
    /// Doc comment for Foo...
    /// </summary>
    enum Foo
    {
        Goo = 0,
        Hoo,
        Joo,
        Koo,
        Loo
    }
}";

            var result =
@"using System;

namespace ConsoleApplication1
{
    /// <summary>
    /// Doc comment for Foo...
    /// </summary>
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

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute_AddsUsingSystemWhenUsingSystemDotAnything()
        {
            var original =
@"using System.Text;

namespace ConsoleApplication1
{
    enum Foo
    {
    }
}";

            var result =
@"using System.Text;
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
    }
}";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_InspectionDoesNotReturnWhenFlagsAlreadyApplied()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_InspectionDoesNotReturnWhenFlagsAttributeAlreadyApplied()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [FlagsAttribute]
    enum Foo
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_InspectionDoesNotReturnWhenFlagsAlreadyAppliedAsChain()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete(""I'm obsolete""), Flags]
    enum Foo
    {
    }
}";

            VerifyDiagnostic(original);
        }
    }
}