using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Arithmetic.DivideIntegerByInteger;

namespace VSDiagnostics.Test.Tests.Arithmetic
{
    [TestClass]
    public class DivideIntegerByIntegerTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new DivideIntegerByIntegerAnalyzer();

        [TestMethod]
        public void DivideIntegerByInteger_TwoIntegers()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                int result = 5 / 6;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "5 / 6"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_IntegerAndDouble()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5 / 6.0;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DivideIntegerByInteger_DoubleAndInteger()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5.0 / 6;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DivideIntegerByInteger_DoubleAndDouble()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5.0 / 6.0;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DivideIntegerByInteger_ThreeIntegerOperands()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5 / 6 / 2;
            }
        }
    }";

            VerifyDiagnostic(original,
                string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "5 / 6 / 2"),
                string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "5 / 6"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_ThreeIntegerOperands_TwoSubsequentIntegers_IntegerDivisionEvaluatedFirst()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5 / 6 / 2.0;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "5 / 6"));
        }

        /// <summary>
        /// This scenario is less important because 5.0 / 6 is evaluated first and results in a double.
        /// In practice there will be no integer division here.
        /// </summary>
        [TestMethod]
        public void DivideIntegerByInteger_ThreeIntegerOperands_TwoSubsequentIntegers_IntegerDivisionEvaluatedLast()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5.0 / 6 / 2;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DivideIntegerByInteger_ThreeIntegerOperands_NoSubsequentIntegerOperands()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                double result = 5 / 6.0 / 2;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DivideIntegerByInteger_DynamicOperand()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                dynamic x = 5;
                double result = x / 3;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void DivideIntegerByInteger_TwoIntegers_OneAsVariable()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                var x = 5;
                double result = x / 3;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / 3"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_TwoIntegers_TwoAsVariables()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                var x = 5;
                var y = 6;
                double result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_TwoIntegers_MethodReference()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                var y = 6;
                double result = IntMethod() / y;
            }

            int IntMethod()
            {
                return 5;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "IntMethod() / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_TwoIntegers_InsideExpression()
        {
            var original = @"
    using System;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                Console.WriteLine(5 / 6);
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "5 / 6"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_ShortAndShort()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                short x = 5;
                short y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_LongAndLong()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                long x = 5;
                long y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_LongAndInt()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                long x = 5;
                int y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_ULongAndULong()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                ulong x = 5;
                ulong y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_UIntAndUInt()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                uint x = 5;
                uint y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_UShortAndUShort()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                ushort x = 5;
                ushort y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_ByteAndByte()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                byte x = 5;
                byte y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }

        [TestMethod]
        public void DivideIntegerByInteger_SByteAndSByte()
        {
            var original = @"
    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
                sbyte x = 5;
                sbyte y = 6;
                var result = x / y;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(DivideIntegerByIntegerAnalyzer.Rule.MessageFormat.ToString(), "x / y"));
        }
    }
}
