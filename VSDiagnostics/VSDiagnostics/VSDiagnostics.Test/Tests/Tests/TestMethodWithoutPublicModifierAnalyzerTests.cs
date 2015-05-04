using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifier;

namespace VSDiagnostics.Test.Tests.Tests
{
    [TestClass]
    public class TestMethodWithoutPublicModifierAnalyzerTests : CodeFixVerifier
    {
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

            VerifyCSharpDiagnostic(test);
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

            VerifyCSharpDiagnostic(original);
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

            VerifyCSharpDiagnostic(original);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TestMethodWithoutPublicModifierAnalyzer.DiagnosticId,
                Message = string.Format(TestMethodWithoutPublicModifierAnalyzer.Message, "Method"),
                Severity = TestMethodWithoutPublicModifierAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TestMethodWithoutPublicModifierAnalyzer.DiagnosticId,
                Message = string.Format(TestMethodWithoutPublicModifierAnalyzer.Message, "Method"),
                Severity = TestMethodWithoutPublicModifierAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TestMethodWithoutPublicModifierAnalyzer.DiagnosticId,
                Message = string.Format(TestMethodWithoutPublicModifierAnalyzer.Message, "Method"),
                Severity = TestMethodWithoutPublicModifierAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 9)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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

            VerifyCSharpDiagnostic(original);
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
            protected internal void Method()
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TestMethodWithoutPublicModifierAnalyzer.DiagnosticId,
                Message = string.Format(TestMethodWithoutPublicModifierAnalyzer.Message, "Method"),
                Severity = TestMethodWithoutPublicModifierAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TestMethodWithoutPublicModifierAnalyzer.DiagnosticId,
                Message = string.Format(TestMethodWithoutPublicModifierAnalyzer.Message, "Method"),
                Severity = TestMethodWithoutPublicModifierAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TestMethodWithoutPublicModifierAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TestMethodWithoutPublicModifierCodeFix();
        }
    }
}