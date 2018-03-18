using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.RedundantPrivateSetter;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class RedundantPrivateSetterTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new RedundantPrivateSetterAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new RedundantPrivateSetterCodeFix();

        [TestMethod]
        public void RedundantPrivateSetter()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; private set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; }
    }
}";

            VerifyDiagnostic(original,
                string.Format(RedundantPrivateSetterAnalyzer.Rule.MessageFormat.ToString(), "MyProperty"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantPrivateSetter_ConstructorUsage()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; private set; }

        public MyClass()
        {
            MyProperty = 42;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; }

        public MyClass()
        {
            MyProperty = 42;
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(RedundantPrivateSetterAnalyzer.Rule.MessageFormat.ToString(), "MyProperty"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantPrivateSetter_NonConstructorUsage()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; private set; }

        public void MyMethod()
        {
            MyProperty = 42;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_ConstructorAndNonConstructorUsage()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; private set; }

        public MyClass()
        {
            MyProperty = 42;
        }

        public void Method()
        {
            MyProperty = 69;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_NonPrivateAccessibility()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; protected set; }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_NoAccessibility()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; set; }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_NoSetter()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_ExpressionBodiedProperty()
        {
            var original = @"
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
        public void RedundantPrivateSetter_NonConstructorReading()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; private set; }

        public void MyMethod()
        {
            if(MyProperty > 5)
            {
                
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public int MyProperty { get; }

        public void MyMethod()
        {
            if(MyProperty > 5)
            {
                
            }
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(RedundantPrivateSetterAnalyzer.Rule.MessageFormat.ToString(), "MyProperty"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void RedundantPrivateSetter_PartialClass()
        {
            var firstTree = @"
namespace ConsoleApplication1
{
    partial class MyClass
    {
        public int MyProperty { get; private set; }

        public MyClass()
        {
            MyProperty = 42;
        }
    }
}";

            var secondTree = @"
namespace ConsoleApplication1
{
    partial class MyClass
    {
        public void MyMethod()
        {
            MyProperty = 42;
        }
    }
}";

            VerifyDiagnostic(new[] {firstTree, secondTree});
        }

        [TestMethod]
        public void RedundantPrivateSetter_PartialClass_IrrelevantIdentifier()
        {
            var firstTree = @"
namespace ConsoleApplication1
{
    partial class MyClass
    {
        public int MyProperty { get; private set; }

        public MyClass()
        {
            MyProperty = 42;
        }
    }
}";

            var secondTree = @"
namespace ConsoleApplication1
{
    partial class MyClass
    {
        public void MyMethod()
        {
            var MyProperty = 42;
        }
    }
}";

            VerifyDiagnostic(new[] { firstTree, secondTree }, string.Format(RedundantPrivateSetterAnalyzer.Rule.MessageFormat.ToString(), "MyProperty"));
        }

        [TestMethod]
        public void RedundantPrivateSetter_StaticNonConstructorUsage()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        public static int MyProperty { get; private set; }

        public static void MyMethod()
        {
            MyProperty = 42;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_Indexer()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
	    public string this[int i]
	    {
		    get
		    {
			    return string.Empty;
		    }

		    private set { }
	    }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void RedundantPrivateSetter_Struct()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        public int MyProperty { get; private set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        public int MyProperty { get; }
    }
}";

            VerifyDiagnostic(original,
                string.Format(RedundantPrivateSetterAnalyzer.Rule.MessageFormat.ToString(), "MyProperty"));
            VerifyFix(original, result);
        }
    }
}