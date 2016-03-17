using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.VisualBasic;
using VSDiagnostics.Diagnostics.Attributes.EnumCanHaveFlagsAttribute;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class EnumCanHaveFlagsAttributeVisualBasicTests : VisualBasicCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EnumCanHaveFlagsAttributeAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new EnumCanHaveFlagsAttributeCodeFix();

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute()
        {
            var original = @"
Module Module1

    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            var result = @"
Imports System
Module Module1
    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_AddsFlagsAttribute_OnlyAddsFlagsAttribute()
        {
            var original = @"
Imports System
Module Module1

    <Obsolete(""I'm obsolete"")>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            var result = @"
Imports System
Module Module1

    <Obsolete(""I'm obsolete"")>
    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_EnumHasXmlDocComment_OnlyAddsFlagsAttribute()
        {
            var original = @"
Module Module1

    ''' <summary>
    ''' Doc comment for Foo...
    ''' </summary>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            var result = @"
Imports System
Module Module1

    ''' <summary>
    ''' Doc comment for Foo...
    ''' </summary>
    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original, EnumCanHaveFlagsAttributeAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_InspectionDoesNotReturnWhenFlagsAlreadyApplied()
        {
            var original = @"
Imports System
Module Module1

    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_InspectionDoesNotReturnWhenFlagsAttributeAlreadyApplied()
        {
            var original = @"
Imports System
Module Module1

    <FlagsAttribute>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EnumCanHaveFlagsAttribute_InspectionDoesNotReturnWhenFlagsAlreadyAppliedAsChain()
        {
            var original = @"
Imports System
Module Module1

    <Obsolete(""I'm obsolete""), Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original);
        }
    }
}