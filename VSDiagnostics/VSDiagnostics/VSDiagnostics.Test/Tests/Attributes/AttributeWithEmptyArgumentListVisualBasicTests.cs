using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.VisualBasic;
using VSDiagnostics.Diagnostics.Attributes.AttributeWithEmptyArgumentList;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class AttributeWithEmptyArgumentListTests : VisualBasicCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AttributeWithEmptyArgumentListAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new AttributeWithEmptyArgumentListCodeFix();

        [TestMethod]
        public void AttributeWithEmptyArgumentList_AttributeWithEmptyArgumentList()
        {
            var original = @"
Module Module1

    <Obsolete()>
    Sub Foo()

    End Sub

End Module";

            var result = @"
Module Module1

    <Obsolete>
    Sub Foo()

    End Sub

End Module";

            VerifyDiagnostic(original, AttributeWithEmptyArgumentListAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AttributeWithEmptyArgumentList_WithoutArgumentList()
        {
            var original = @"
Module Module1

    <Obsolete>
    Sub Foo()

    End Sub

End Module";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AttributeWithEmptyArgumentList_WithArgumentList()
        {
            var original = @"
Module Module1

    <Obsolete(""test"", true)>
    Sub Foo()

    End Sub

End Module";

            VerifyDiagnostic(original);
        }

        // make sure it works on other attributes besides [Obsolete]
        [TestMethod]
        public void AttributeWithEmptyArgumentList_AttributeWithEmptyArgumentList_FlagsAttribute()
        {
            var original = @"
Module Module1
    
    <Flags()>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            var result = @"
Module Module1
    
    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original, AttributeWithEmptyArgumentListAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        // make sure it works on other attributes besides [Obsolete]
        [TestMethod]
        public void AttributeWithEmptyArgumentList_WithoutArgumentList_FlagsAttribute()
        {
            var original = @"
Module Module1
    
    <Flags>
    Enum Foo
        Bar
        Baz
    End Enum

End Module";

            VerifyDiagnostic(original);
        }
    }
}