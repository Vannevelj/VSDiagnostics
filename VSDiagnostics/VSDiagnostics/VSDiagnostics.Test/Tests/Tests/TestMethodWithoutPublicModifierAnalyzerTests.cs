using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifier;

namespace VSDiagnostics.Test.Tests.Tests
{
    [TestClass]
    public class TestMethodWithoutPublicModifierAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new TestMethodWithoutPublicModifierAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new TestMethodWithoutPublicModifierCodeFix();

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithPublicModifierAndTestAttribute_DoesNotInvokeWarning()
        {
            var test = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        [TestFixture]
        public class MyClass
        {   
            [Test]
            public void Method()
            {
                
            }
        }
    }";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithPublicModifierAndTestMethodAttribute_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        [TestClass]
        public class MyClass
        {   
            [TestMethod]
            public void Method()
            {
                
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithPublicModifierAndFactAttribute_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        public class MyClass
        {   
            [Fact]
            public void Method()
            {
                
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithInternalModifierAndTestAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestFixture]
    public class MyClass
    {   
        [Test]
        internal void Method()
        {
                
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestFixture]
    public class MyClass
    {   
        [Test]
        public void Method()
        {
                
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TestMethodWithoutPublicModifierAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithInternalModifierAndTestMethodAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestClass]
    public class MyClass
    {
        [TestMethod]
        internal void Method()
        {

        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestClass]
    public class MyClass
    {
        [TestMethod]
        public void Method()
        {

        }
    }
}";

            VerifyDiagnostic(original, string.Format(TestMethodWithoutPublicModifierAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithInternalModifierAndFactAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    public class MyClass
    {
        [Fact]
        internal void Method()
        {
                
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    public class MyClass
    {
        [Fact]
        public void Method()
        {
                
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TestMethodWithoutPublicModifierAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithPublicModifierAndMultipleAttributes_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        [TestFixture]
        public class MyClass
        {
            [Ignore]
            [Test]
            public void Method()
            {
                
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithProtectedInternalModifierAndTestMethodAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestClass]
    public class MyClass
    {
        [TestMethod]
        protected internal virtual void Method()
        {

        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestClass]
    public class MyClass
    {
        [TestMethod]
        public virtual void Method()
        {

        }
    }
}";

            VerifyDiagnostic(original, string.Format(TestMethodWithoutPublicModifierAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithMultipleModifiersAndTestMethodAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestClass]
    public class MyClass
    {
        [TestMethod]
        internal virtual void Method()
        {

        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestClass]
    public class MyClass
    {
        [TestMethod]
        public virtual void Method()
        {

        }
    }
}";

            VerifyDiagnostic(original, string.Format(TestMethodWithoutPublicModifierAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithoutModifierAndTestAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestFixture]
    public class MyClass
    {   
        [Test]
        void Method()
        {
                
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    [TestFixture]
    public class MyClass
    {   
        [Test]
        public void Method()
        {
                
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TestMethodWithoutPublicModifierAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithoutTestAttributeAttribute_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        public class MyClass
        {   
            private static void Method()
            {
                
            }
        }
    }";

            VerifyDiagnostic(original);
        }
    }
}