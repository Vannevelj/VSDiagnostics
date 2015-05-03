using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.Tests.TestMethodWithoutPublicModifierAnalyzer;

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
            var test = @"
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

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithPublicModifierAndFactAttribute_DoesNotInvokeWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithInternalModifierAndTestAttribute_InvokesWarning()
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
            internal void Method()
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithInternalModifierAndTestMethodAttribute_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithInternalModifierAndFactAttribute_InvokesWarning()
        {
            var test = @"
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TestMethodWithoutPublicModifierAnalyzer.DiagnosticId,
                Message = string.Format(TestMethodWithoutPublicModifierAnalyzer.Message, "Method"),
                Severity = TestMethodWithoutPublicModifierAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void TestMethodWithoutPublicModifierAnalyzer_WithPublicModifierAndMultipleAttributes_DoesNotInvokeWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TestMethodWithoutPublicModifierAnalyzer();
        }
    }
}