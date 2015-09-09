using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.VisualBasic;
using VSDiagnostics.Diagnostics.Attributes.ObsoleteAttributeWithoutReason;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class ObsoleteAttributeWithoutReasonVisualBasicTests : VisualBasicDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ObsoleteAttributeWithoutReasonAnalyzer();

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithNullArgumentList()
        {
            var test = @"
Module Module1

    <Obsolete>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithoutReason()
        {
            var test = @"
Module Module1

    <ObsoleteAttribute>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithEmptyArgumentList()
        {
            var test = @"
Module Module1

    <Obsolete()>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithEmptyArgumentList()
        {
            var test = @"
Module Module1

    <ObsoleteAttribute()>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithArgument()
        {
            var test = @"
Module Module1

    <Obsolete(""I have an argument."")>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithArgument()
        {
            var test = @"
Module Module1

    <ObsoleteAttribute(""I have an argument."")>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithArguments()
        {
            var test = @"
Module Module1

    <Obsolete(""I have two arguments."", true)>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithArguments()
        {
            var test = @"
Module Module1

    <ObsoleteAttribute(""I have two arguments."", true)>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_NonObsoleteAttribute()
        {
            var test = @"
Module Module1

    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(test);
        }
    }
}