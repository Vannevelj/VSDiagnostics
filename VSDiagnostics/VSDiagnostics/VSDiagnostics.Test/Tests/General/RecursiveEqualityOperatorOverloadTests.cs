using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.RecursiveEqualityOperatorOverload;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class RecursiveEqualityOperatorOverloadTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RecursiveOperatorOverloadAnalyzer();

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithEqualityOperators()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ==(A a1, A a2) 
	    {
		    return a1 == a2;
	    }

	    public static A operator !=(A a1, A a2)
	    {
		    return a1 != a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithEquals()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
        public static A operator ==(A a1, A a2) 
        {
	        var a = a1.Equals(a2);
		    return a1;
        }

	    public static A operator !=(A a1, A a2)
	    {
		    var a = a1.Equals(a2);
		    return a1;
	    }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithDifferentComparison()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
        public static A operator ==(A a1, A a2) 
        {
	        var a = 1 == 1;
		    return a1;
        }

	    public static A operator !=(A a1, A a2)
	    {
		    var a = 1 == 1;
		    return a1;
	    }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithDifferentOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
        public static A operator ==(A a1, A a2) 
        {
	        return a1 + a2;
        }

	    public static A operator !=(A a1, A a2)
	    {
		    return a1 + a2;
	    }

        public static A operator +(A a1, A a2)
	    {
		    return a1;
	    }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithNullComparison()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ==(A a1, A a2) 
	    {
		    return a1 == null;
	    }

	    public static A operator !=(A a1, A a2)
	    {
		    return a1 != null;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithNullComparisonLeftHand()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ==(A a1, A a2) 
	    {
		    return null == a2;
	    }

	    public static A operator !=(A a1, A a2)
	    {
		    return null != a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithIs()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static bool operator ==(A a1, A a2)
	    {
		    return a1 is A;
	    }

	    public static bool operator !=(A a1, A a2)
	    {
		    return !(a1 is A);
	    }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithReturnNull()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ==(A a1, A a2)
	    {
		    return null;
	    }

	    public static A operator !=(A a1, A a2)
	    {
		    return null;
	    }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithExpressionBody()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator +(A a1, A a2) => a1 + a2;
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithPlusOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator +(A a1, A a2)
	    {
		    return a1 + a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithUnaryPlusOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator +(A a1)
	    {
		    return +a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithMinusOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator -(A a1, A a2)
	    {
		    return a1 - a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithUnaryMinusOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator -(A a1)
	    {
		    return -a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithMultiplicationOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator *(A a1, A a2)
	    {
		    return a1 * a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithDivisionOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator /(A a1, A a2)
	    {
		    return a1 / a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithNotOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator !(A a1)
	    {
		    return !a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithBitwiseNotnOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ~(A a1)
	    {
		    return ~a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithPostFixIncrementOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ++(A a1)
	    {
		    return a1++;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithPreFixIncrementOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ++(A a1)
	    {
		    return ++a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithDecrementOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator --(A a1)
	    {
		    return --a1;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithTrueAndFalseOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static bool operator true(A a1)
	    {
		    if (a1)
			    return true;
		    else
			    return false;
	    }

	    public static bool operator false(A a1)
	    {
		    if (a1)
			    return false;
		    else
			    return true;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithTrueAndFalseOperatorAsExpressionBody()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static bool operator true(A a1) => a1 ? true : false;

	    public static bool operator false(A a1) => a1 ? false : true;
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithTrueAndFalseOperatorAsExpressionBodyWithNestedConditionals()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static bool operator true(A a1) => a1 ? true : a1 ? false : true;

	    public static bool operator false(A a1) => a1 ? false : a1 ? true : false;
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator", "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithLeftShiftOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator <<(A a1, int a2)
	    {
		    return a1 << 5;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithRightShiftOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator >>(A a1, int a2)
	    {
		    return a1 >> 5;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithXorOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator ^(A a1, A a2)
	    {
		    return a1 ^ a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithOrOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator |(A a1, A a2)
	    {
		    return a1 | a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithAndOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator &(A a1, A a2)
	    {
		    return a1 & a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithModOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator %(A a1, A a2)
	    {
		    return a1 % a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithGreaterLesserThanEqualityOperators()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator >=(A a1, A a2) 
	    {
		    return a1 >= a2;
	    }

	    public static A operator <=(A a1, A a2)
	    {
		    return a1 <= a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithCastOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
        public static implicit operator string(A a) 
        {
	        return ""test"";
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RecursiveEqualityOperatorOverload_WithMultipleOperators()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class A
    {
	    public static A operator +(A a1, A a2)
	    {
            var a = a1 + a2;
		    return a + a2;
	    }
    }
}";

            VerifyDiagnostic(original, "Recursively using overloaded operator", "Recursively using overloaded operator");
        }
    }
}
