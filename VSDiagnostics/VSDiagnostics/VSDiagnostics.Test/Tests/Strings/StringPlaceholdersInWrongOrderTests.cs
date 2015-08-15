using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Strings.StringPlaceholdersInWrongOrder;

namespace VSDiagnostics.Test.Tests.Strings
{
    [TestClass]
    public class StringPlaceholdersInWrongOrderTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new StringPlaceholdersInWrongOrderAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new StringPlaceHoldersInWrongOrderCodeFix();

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InCorrectOrder_WithSingleOccurrence_DoesNotInvokeWarning()
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
                string s = string.Format(""Hello {0}, my name is {1}"", ""Mr. Test"", ""Mr. Tester"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InCorrectOrder_WithMultipleOccurrences_DoesNotInvokeWarning()
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
                string s = string.Format(""Hello {0}, my name is {1}. Yes you heard that right, {1}."", ""Mr. Test"", ""Mr. Tester"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithMultipleOccurrences_InvokesWarning()
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
                string s = string.Format(""Hello {1}, my name is {0}. Yes you heard that right, {0}."", ""Mr. Test"", ""Mr. Tester"");
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
                string s = string.Format(""Hello {0}, my name is {1}. Yes you heard that right, {1}."", ""Mr. Tester"", ""Mr. Test"");
            }
        }
    }";

            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithSingleOccurrence_InvokesWarning()
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
                string s = string.Format(""Hello {1}, my name is {0}."", ""Mr. Test"", ""Mr. Tester"");
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
                string s = string.Format(""Hello {0}, my name is {1}."", ""Mr. Tester"", ""Mr. Test"");
            }
        }
    }";

            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithUnusedPlaceholder_InvokesWarning()
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
                string s = string.Format(""Hello {2}, my name is {1}. Yes you heard that right, {1}."", ""Mr. Test"", ""Mr. Tester"", ""Mrs. Testing"");
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
                string s = string.Format(""Hello {0}, my name is {1}. Yes you heard that right, {1}."", ""Mrs. Testing"", ""Mr. Tester"", ""Mr. Test"");
            }
        }
    }";

            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithMultiplePlaceholders_InvokesWarning()
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
                string s = string.Format(""abc {2} def {0} ghi {1}"", ""x"", ""y"", ""z"");
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
                string s = string.Format(""abc {0} def {1} ghi {2}"", ""z"", ""x"", ""y"");
            }
        }
    }";

            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithSinglePlaceholder_DoesNotInvokeWarning()
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
                string s = string.Format(""abc {1}"", ""x"", ""y"", ""z"");
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_WithFormatDefinedSeparately_DoesNotInvokeWarning()
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
                string format = ""Hello {0}, my name is {1}"";
                string s = string.Format(format, ""Mr. Test"", ""Mr. Tester"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_WithInterpolatedString_DoesNotInvokeWarning()
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
                string name = ""Jeroen"";
                string s = string.Format($""haha {name}, you're so {0}!"", ""funny"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithFormattedString_InvokesWarning()
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
                DateTime date = DateTime.Now;
                string formattedDate = string.Format(""Hello {1}, it's {0:hh:mm:ss t z}"", date, ""Jeroen"");
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
                DateTime date = DateTime.Now;
                string formattedDate = string.Format(""Hello {0}, it's {1:hh:mm:ss t z}"", ""Jeroen"", date);
            }
        }
    }";

            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithFormatProvider_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Globalization;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                string s = string.Format(CultureInfo.InvariantCulture, ""Hello {1}, my name is {0}."", ""Mr. Test"", ""Mr. Tester"");
            }
        }
    }";

            var expected = @"
    using System;
    using System.Text;
    using System.Globalization;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            void Method()
            {
                string s = string.Format(CultureInfo.InvariantCulture, ""Hello {0}, my name is {1}."", ""Mr. Tester"", ""Mr. Test"");
            }
        }
    }";

            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_WithEscapedCurlyBrace_DoesNotInvokeWarning()
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
                string s = string.Format(""Hello {{Jeroen}}, my name is {0}"", ""Mr. Test"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_WithDoubleEscapedCurlyBrace_InvokesWarning()
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
                string s = string.Format(""Hello {{{1}}}, my name is {0}"", ""Mr. Test"", ""Mr. Tester"");
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
                string s = string.Format(""Hello {{{0}}}, my name is {1}"", ""Mr. Tester"", ""Mr. Test"");
            }
        }
    }";
            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_WithNestedCurlyBraces_InvokesWarning()
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
                string s = string.Format(""{{Hello {1}, my name is {0}}}"", ""Mr. Test"", ""Mr. Tester"");
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
                string s = string.Format(""{{Hello {0}, my name is {1}}}"", ""Mr. Tester"", ""Mr. Test"");
            }
        }
    }";
            VerifyDiagnostic(original, StringPlaceholdersInWrongOrderAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_WithCommentedPlaceholder_AlsoUsedValidly_DoesNotInvokeWarning()
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
                string s = string.Format(""Hello {{0}}, my name is {0}."", ""Mr. Test"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringPlaceholdersInWrongOrder_InIncorrectOrder_WithInvalidIndex_DoesNotInvokeWarning()
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
                string s = string.Format(""{0} {1} {4} {3}"", ""a"", ""b"", ""c"", ""d"");
            }
        }
    }";
            VerifyDiagnostic(original);
        }
    }
}