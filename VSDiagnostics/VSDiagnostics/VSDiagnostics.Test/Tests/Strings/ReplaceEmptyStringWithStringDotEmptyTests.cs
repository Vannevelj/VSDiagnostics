using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty;

namespace VSDiagnostics.Test.Tests.Strings
{
    [TestClass]
    public class ReplaceEmptyStringWithStringDotEmptyTests : CodeFixVerifier
    {
        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithLocalEmptyStringLiteral_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                string s = """";
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                string s = string.Empty;
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ReplaceEmptyStringWithStringDotEmptyAnalyzer.DiagnosticId,
                Message = ReplaceEmptyStringWithStringDotEmptyAnalyzer.Message,
                Severity = ReplaceEmptyStringWithStringDotEmptyAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 28)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithDefaultParameterEmptyStringLiteral_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method(string s = """")
            {
                
            }
        }
    }";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithNonEmptyStringLiteral_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                string s = ""hello world"";
            }
        }
    }";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithStringLiteralAsArgument_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                Method2("""");
            }

            void Method2(string s)
            {

            }
        }
    }";

            var result = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                Method2(string.Empty);
            }

            void Method2(string s)
            {

            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ReplaceEmptyStringWithStringDotEmptyAnalyzer.DiagnosticId,
                Message = ReplaceEmptyStringWithStringDotEmptyAnalyzer.Message,
                Severity = ReplaceEmptyStringWithStringDotEmptyAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 25)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithStringDotEmpty_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                string s = string.Empty;
            }
        }
    }";

            VerifyCSharpDiagnostic(original);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ReplaceEmptyStringWithStringDotEmptyCodeFix();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ReplaceEmptyStringWithStringDotEmptyAnalyzer();
        }
    }
}