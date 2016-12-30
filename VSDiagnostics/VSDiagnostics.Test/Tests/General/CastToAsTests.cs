using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.CastToAs;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class CastToAsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CastToAsAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new CastToAsCodeFix();

        [TestMethod]
        public void CastToAs_ValueType()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            char ch = 'r';
            int i = (int) ch;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CastToAs_NullableValueType()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool b = true;
            object o = b;
            bool? b2 = (bool?)o;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool b = true;
            object o = b;
            bool? b2 = o as bool?;
        }
    }
}";

            VerifyDiagnostic(original, CastToAsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CastToAs_CustomType()
        {
            var original = @"
namespace ConsoleApplication1
{
    interface P
    {
    }

    class Program : P
    {
    }

    class MyClass
    {
        void Method()
        {
            P variable = new Program();
            var i = (Program) variable;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    interface P
    {
    }

    class Program : P
    {
    }

    class MyClass
    {
        void Method()
        {
            P variable = new Program();
            var i = variable as Program;
        }
    }
}";

            VerifyDiagnostic(original, CastToAsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CastToAs_MethodCall()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Main()
        {
            bool? b = (bool?)GetBoxedType();
        }

        object GetBoxedType()
        {
            return true;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Main()
        {
            bool? b = GetBoxedType() as bool?;
        }

        object GetBoxedType()
        {
            return true;
        }
    }
}";

            VerifyDiagnostic(original, CastToAsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CastToAs_FormatsOnlySpecificNode()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Main()
        {
            bool? b = (bool?) GetBoxedType();
        }

        object GetBoxedType ()  // make sure this space doesn't get fixed
        {
            return true;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Main()
        {
            bool? b = GetBoxedType() as bool?;
        }

        object GetBoxedType ()  // make sure this space doesn't get fixed
        {
            return true;
        }
    }
}";

            VerifyDiagnostic(original, CastToAsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CastToAs_Generics()
        {
            var original = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    static class Extensions
    {
	    public static void Method<T>(this IEnumerable<T> input) where T : class
	    {
		    var list = new List<T>();
		
		    foreach (var node in input)
		    {
			    list.Add((T)node);
		    }
	    }
    }
}";

            var result = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    static class Extensions
    {
	    public static void Method<T>(this IEnumerable<T> input) where T : class
	    {
		    var list = new List<T>();
		
		    foreach (var node in input)
		    {
			    list.Add(node as T);
		    }
	    }
    }
}";

            VerifyDiagnostic(original, CastToAsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CastToAs_Generics_ValueType()
        {
            var original = @"
using System.Collections.Generic;

namespace ConsoleApplication1
{
    static class Extensions
    {
	    public static void Method<T>(this IEnumerable<T> input) where T : struct
	    {
		    var list = new List<T>();
		
		    foreach (var node in input)
		    {
			    list.Add((T)node);
		    }
	    }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}