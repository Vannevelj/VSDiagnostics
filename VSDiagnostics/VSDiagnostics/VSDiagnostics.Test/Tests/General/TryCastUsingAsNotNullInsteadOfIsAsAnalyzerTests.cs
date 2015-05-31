using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.TryCastUsingAsNotNullInsteadOfIsAs;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TryCastUsingAsNotNullInsteadOfIsAsAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void TryCastUsingAsNotNullInsteadOfIsAsAnalyzer_WithIsAs_AndReferenceType_InvokesWarning()
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
            object o = ""sample"";
            if (o is string)
            {
                int oAsInt = o as string;
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.DiagnosticId,
                Message = string.Format(TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Message, "o"),
                Severity = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastUsingAsNotNullInsteadOfIsAsAnalyzer_WithIsAs_AndValueType_InvokesWarning()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.DiagnosticId,
                Message = string.Format(TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Message, "o"),
                Severity = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastUsingAsNotNullInsteadOfIsAsAnalyzer_WithIsAs_AndObjectIsUsedBeforeIs_InvokesWarning()
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
            object o = ""sample"";
            Console.Write(o.GetType());
            if (o is string)
            {
                int oAsInt = o as string;
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.DiagnosticId,
                Message = string.Format(TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Message, "o"),
                Severity = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastUsingAsNotNullInsteadOfIsAsAnalyzer_WithIsAs_AndObjectIsMethodParameter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                int oAsInt = o as string;
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.DiagnosticId,
                Message = string.Format(TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Message, "o"),
                Severity = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastUsingAsNotNullInsteadOfIsAsAnalyzer_WithIsAs_AndElseClause_InvokesWarning()
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
            object o = ""sample"";
            if (o is string)
            {
                int oAsInt = o as string;
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.DiagnosticId,
                Message = string.Format(TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Message, "o"),
                Severity = TryCastUsingAsNotNullInsteadOfIsAsAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TryCastUsingAsNotNullInsteadOfIsAsAnalyzer();
        }
    }
}