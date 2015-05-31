using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TryCastUsingAsNotNullInsteadOfIsAsAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithIsAs_AndReferenceType_InvokesWarning()
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
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithIsAs_AndValueType_InvokesWarning()
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
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithIsAs_AndObjectIsUsedBeforeIs_InvokesWarning()
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
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithIsAs_AndObjectIsMethodParameter_InvokesWarning()
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
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithIsAs_AndElseClause_InvokesWarning()
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
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithMultipleCasts_InvokesWarning()
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
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithDirectCast_InvokesWarning()
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
                var oAsInt = (int) o;
            }
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TryCastWithoutUsingAsNotNullAnalyzer.DiagnosticId,
                Message = string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Message, "o"),
                Severity = TryCastWithoutUsingAsNotNullAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithoutCorrespondingCast_DoesNotInvokeWarning()
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
            object irrelevant = 10.0;
            if (o is int)
            {
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyCSharpDiagnostic(original);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TryCastWithoutUsingAsNotNullAnalyzer();
        }
    }
}