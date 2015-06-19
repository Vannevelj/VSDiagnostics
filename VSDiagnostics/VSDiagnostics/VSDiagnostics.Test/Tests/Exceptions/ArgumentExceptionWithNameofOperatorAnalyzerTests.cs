using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithNameofOperator;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class ArgumentExceptionWithNameofOperatorAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ArgumentExceptionWithNameofOperatorAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ArgumentExceptionWithNameofOperatorCodeFix();

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
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 49)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithoutCorrespondingParameter_DoesNotInvokeWarning()
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithoutCorrespondingParameter_ButDefinedOutsideMethodScope_DoesNotInvokeWarning()
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithTwoOccurrences_InvokesTwoWarnings()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input, int otherInput)
        {
            if(input == null)
                throw new ArgumentException(""input"");

            if(otherInput == null)
                throw new ArgumentException(""otherInput"");
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
        void Method(string input, int otherInput)
        {
            if(input == null)
                throw new ArgumentException(nameof(input));

            if(otherInput == null)
                throw new ArgumentException(nameof(otherInput));
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 45)
                    }
            };

            var secondExpectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "otherInput"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 15, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, secondExpectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithIntType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(int input)
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
            void Method(int input)
            {
                throw new ArgumentException(nameof(input));
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithDefaultValue_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(double input = 5.7)
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
            void Method(double input = 5.7)
            {
                throw new ArgumentException(nameof(input));
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.Id,
                Message = string.Format(ArgumentExceptionWithNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                Severity = ArgumentExceptionWithNameofOperatorAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 45)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }
    }
}