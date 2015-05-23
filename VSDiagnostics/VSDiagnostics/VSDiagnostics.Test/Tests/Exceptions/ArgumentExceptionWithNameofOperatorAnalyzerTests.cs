using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithNameofOperator;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class ArgumentExceptionWithNameofOperatorAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithoutCorrespondingParameter_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentException(""input"");
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
            void Method(string input)
            {
                throw new ArgumentException(nameof(input));
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.DiagnosticId,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Message, "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithoutCorrespondingParameterInDifferentCase_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentException(""InPut"");
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
            void Method(string input)
            {
                throw new ArgumentException(nameof(input));
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.DiagnosticId,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Message, "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithMultipleArguments_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentException(""input"", new Exception());
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
            void Method(string input)
            {
                throw new ArgumentException(nameof(input), new Exception());
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.DiagnosticId,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Message, "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithMultipleParameters_AndCorrespondingParameter_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input, int anotherInput)
            {
                throw new ArgumentException(""input"");
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
            void Method(string input, int anotherInput)
            {
                throw new ArgumentException(nameof(input));
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.DiagnosticId,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Message, "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithCorrespondingParameterAsString_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentNullException(""input"");
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
            void Method(string input)
            {
                throw new ArgumentNullException(nameof(input));
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.DiagnosticId,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Message, "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 23)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithCorrespondingParameterAsNameOf_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentNullException(nameof(input));
            }
        }
    }";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithOutCorrespondingParameter_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(string input)
            {
                throw new ArgumentNullException(""somethingElse"");
            }
        }
    }";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithOutCorrespondingParameter_ButDefinedOutsideMethodScope_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            private ArgumentException _exception = new ArgumentException(""input"");

            void Method(string input)
            {
                throw _exception;
            }
        }
    }";

            VerifyCSharpDiagnostic(original);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ArgumentExceptionWithNameofOperatorAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ArgumentExceptionWithNameofOperatorCodeFix();
        }
    }
}