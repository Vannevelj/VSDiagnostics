using Microsoft.CodeAnalysis.CodeFixes;
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
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithCorrespondingParameter()
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
        public void ArgumentExceptionWithoutNameofOperator_WithArgumentException_WithCorrespondingParameterInDifferentCase()
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

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_WithMultipleLevelSubclassException()
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
                throw new MyException(""input"");
            }
        }

        class MyException : ArgumentNullException
        {
	        public MyException(string message) : base(message)
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
            void Method(string input)
            {
                throw new MyException(nameof(input));
            }
        }

        class MyException : ArgumentNullException
        {
	        public MyException(string message) : base(message)
	        {
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "input"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_PropertySetter()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            public int MyProperty 
            { 
                get { return 5; }
                set { throw new ArgumentException(""value""); }
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
            public int MyProperty 
            { 
                get { return 5; }
                set { throw new ArgumentException(nameof(value)); }
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(ArgumentExceptionWithoutNameofOperatorAnalyzer.Rule.MessageFormat.ToString(), "value"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_PropertyGetterOnly()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            public int MyProperty 
            { 
                get { return 5; }
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_PropertyExpressionBodied()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            public int MyProperty => 5;
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ArgumentExceptionWithoutNameofOperator_PropertySetterDoesNotReferToValue()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            public int MyProperty 
            { 
                get { return 5; }
                set { throw new ArgumentNullException(""test""); }
            }
        }
    }";

            VerifyDiagnostic(original);
        }
    }
}