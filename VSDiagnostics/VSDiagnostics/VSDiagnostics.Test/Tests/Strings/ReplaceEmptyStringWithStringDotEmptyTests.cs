using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty;

namespace VSDiagnostics.Test.Tests.Strings
{
    [TestClass]
    public class ReplaceEmptyStringWithStringDotEmptyTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ReplaceEmptyStringWithStringDotEmptyAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new ReplaceEmptyStringWithStringDotEmptyCodeFix();

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

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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

            VerifyDiagnostic(original);
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

            VerifyDiagnostic(original);
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

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithEmptyStringAsAttributeArgument_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            [MyAttribute(Test = """")]
            void Method()
            {

            }
        }

        [AttributeUsage(AttributeTargets.All)]
        public class MyAttribute : Attribute
        {
	        public string Test { get; set; }
        }
    }";

            VerifyDiagnostic(original);
        }
    }
}