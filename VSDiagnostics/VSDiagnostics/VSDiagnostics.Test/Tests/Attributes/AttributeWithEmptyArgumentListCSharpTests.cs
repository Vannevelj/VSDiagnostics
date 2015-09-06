using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.AttributeWithEmptyArgumentList;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class AttributeWithEmptyArgumentListCSharpTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AttributeWithEmptyArgumentListAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new AttributeWithEmptyArgumentListCodeFix();

        [TestMethod]
        public void AttributeWithEmptyArgumentList_AttributeWithEmptyArgumentList()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete()]
        void Method(string input)
        {
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(original, AttributeWithEmptyArgumentListAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AttributeWithEmptyArgumentList_WithoutArgumentList()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AttributeWithEmptyArgumentList_WithArgumentList()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete(""test"", true)]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        // make sure it works on other attributes besides [Obsolete]
        [TestMethod]
        public void AttributeWithEmptyArgumentList_AttributeWithEmptyArgumentList_FlagsAttribute()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Flags()]
        enum Foo
        {
            Bar, Baz
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Flags]
        enum Foo
        {
            Bar, Baz
        }
    }
}";

            VerifyDiagnostic(original, AttributeWithEmptyArgumentListAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        // make sure it works on other attributes besides [Obsolete]
        [TestMethod]
        public void AttributeWithEmptyArgumentList_WithoutArgumentList_FlagsAttribute()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Flags]
        enum Foo
        {
            Bar, Baz
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}