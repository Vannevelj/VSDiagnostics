﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithoutNameofOperator;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class ArgumentExceptionWithoutNameofOperatorTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ArgumentExceptionWithoutNameofOperatorAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ArgumentExceptionWithoutNameofOperatorCodeFix();

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithoutCorrespondingParameter()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithoutCorrespondingParameterInDifferentCase()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithMultipleArguments()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithMultipleParameters_AndCorrespondingParameter()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentNullException_WithCorrespondingParameterAsString()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentNullException_WithCorrespondingParameterAsNameOf()
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
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentNullException_WithoutCorrespondingParameter()
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
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentNullException_WithoutCorrespondingParameter_ButDefinedOutsideMethodScope()
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
        public void ArgumentExceptionWithoutNameofOperator_WithTwoOccurrences_InvokesTwoWarnings()
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

            VerifyDiagnostic(original,
                string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"),
                string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "otherInput"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithIntType()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithDefaultValue()
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

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }
    }
}