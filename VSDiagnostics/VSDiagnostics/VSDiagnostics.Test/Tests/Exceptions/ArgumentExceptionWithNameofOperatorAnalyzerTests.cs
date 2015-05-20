using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithNameofOperator;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class ArgumentExceptionWithNameofOperatorAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithoutCorrespondingParameter_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithoutCorrespondingParameterInDifferentCase_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithMultipleArguments_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithMultipleParameters_AndCorrespondingParameter_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithCorrespondingParameterAsString_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentException_WithCorrespondingParameterAsString_InvokesWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithCorrespondingParameterAsNameOf_DoesNotInvokeWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void ArgumentExceptionWithNameofOperatorAnalyzer_WithArgumentNullException_WithOutCorrespondingParameter_DoesNotInvokeWarning()
        {
            var test = @"
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

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ArgumentExceptionWithNameofOperatorAnalyzer();
        }
    }
}