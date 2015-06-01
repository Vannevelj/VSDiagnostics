using Microsoft.CodeAnalysis.CodeFixes;
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
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            string oAsString = o as string;
            if (oAsString != null)
            {
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
            VerifyCSharpFix(original, expected);
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

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt = o as int?;
            if (oAsInt != null)
            {
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
            VerifyCSharpFix(original, expected);
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
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
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
            string oAsString = o as string;
            if (oAsString != null)
            {
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
            VerifyCSharpFix(original, expected);
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
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            string oAsString = o as string;
            if (oAsString != null)
            {
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
            VerifyCSharpFix(original, expected);
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
                string oAsString = o as string;
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            string oAsString = o as string;
            if (oAsString != null)
            {
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
            VerifyCSharpFix(original, expected);
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

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt = o as int?;
            if (oAsInt != null)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
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
            VerifyCSharpFix(original, expected);
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

        [TestMethod]
        public void TryCastWithoutUsingAsNotNullAnalyzer_WithChainedVariableDeclaration_InvokesWarning()
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
                int? oAsInt = o as int?, x = 10;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            int oAsInt = o as int?;
            if (oAsInt != null)
            {
                int? x = 10;
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
            VerifyCSharpFix(original, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TryCastWithoutUsingAsNotNullAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TryCastWithoutUsingAsNotNullCodeFix();
        }
    }
}