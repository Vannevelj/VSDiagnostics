using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SimplifyExpressionBodiedMember;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SimplifyExpressionBodiedMemberTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SimplifyExpressionBodiedMemberAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new SimplifyExpressionBodiedMemberCodeFix();

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithSimpleReturnMethod()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            return 5;
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
        int MyMethod() => 5;
    }
}";

            VerifyDiagnostic(original, string.Format(SimplifyExpressionBodiedMemberAnalyzer.Rule.MessageFormat.ToString(), "Method", "MyMethod"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithSimpleGetterOnlyProperty()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty { get { return ""myString""; } }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty => ""myString"";
    }
}";

            VerifyDiagnostic(original, string.Format(SimplifyExpressionBodiedMemberAnalyzer.Rule.MessageFormat.ToString(), "Property", "MyProperty"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMultiLineGetterOnlyProperty()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty 
        { 
            get 
            {
                return ""myString"";
            } 
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
        string MyProperty => ""myString"";
    }
}";

            VerifyDiagnostic(original, string.Format(SimplifyExpressionBodiedMemberAnalyzer.Rule.MessageFormat.ToString(), "Property", "MyProperty"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMultiStatementGetterOnlyProperty()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string MyProperty 
        { 
            get 
            {
                var newValue = ""myString"" + "".AnotherString"";
                return newValue; 
            } 
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMultiStatementMethod()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            var result = 5 * 5;
            return result;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMethodWithVoidReturn()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void MyMethod()
        {
            var result = 5 * 5;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMethodWithoutReturn()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            while(true)
            {
                Console.WriteLine(""This is a weird feature"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMethodWithExpressionBody()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod() => 5;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithPropertyWithExpressionBody()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty => 5;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMethodWithUnreachableCode()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            return 5;
            int x = 6;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMethodAndTrailingComments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod()
        {
            return 5; /* comments */
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithAutoImplementedGetAndSet()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { get; set; }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithMultipleProperties()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { get { return 5; } }
        int MyProperty2 { get { return 6; } }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty => 5;

        int MyProperty2 => 6;
    }
}";

            VerifyDiagnostic(original,
                string.Format(SimplifyExpressionBodiedMemberAnalyzer.Rule.MessageFormat.ToString(), "Property", "MyProperty"),
                string.Format(SimplifyExpressionBodiedMemberAnalyzer.Rule.MessageFormat.ToString(), "Property", "MyProperty2"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithTrivia_AsProperty()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { /* test */ get { return 5; } /* more test */ }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithTrivia_AsMethod()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyMethod() 
        { 
            /* test */ 
            return 5;
            /* more test */
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithTrailingPropertyTrivia()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { get { return 5; }} // lala
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithAttribute()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int MyProperty { [MyAttribute] get { return 5; }} 
    }

    [AttributeUsage(AttributeTargets.All)]
    class MyAttribute : Attribute
    {   
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithGetAndSetImplementation()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private string _myProperty;
        string MyProperty 
        { 
            get { return _myProperty; } 
            set { _myProperty = value; }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithNestedBlock()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        string Name { get { { return ""ParameterCanBeByValInspection""; } } }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithOnlyIfInMethodBody()
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
            if(5 == 5)
            {
                Console.WriteLine();
                return;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithOnlyUsingInMethodBody()
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
            using(null as IDisposable)
            {
                Console.WriteLine();
                return;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithBlockInMethodBody()
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
            {
                Console.WriteLine();
                return;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithWhileInMethodBody()
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
            while(5 == 5)
            {
                Console.WriteLine();
                return;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithDoWhileInMethodBody()
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
            do
            {
                Console.WriteLine();
                return;
            } while (5 == 5);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithSwitchInMethodBody_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
    enum Foo
    {
        Baz,
        Biz
    }
 
    class MyClass
    {
        Foo Bar { get; set; }
 
        public string Method()
        {
            switch (Bar)
            {
                case Foo.Baz:
                    return ""Geannuleerd"";
                case Foo.Biz:
                    return ""In behandeling"";
                default:
                    return ""Status onbekend"";
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithSwitchInPropertyGetter_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
   enum Foo
   {
       Baz,
       Biz
   }
 
   class MyClass
   {
       Foo Bar { get; set; }
 
       public string BookingStatus
       {
           get
           {
               switch (Bar)
               {
                   case Foo.Baz:
                       return ""Geannuleerd"";
                   case Foo.Biz:
                       return ""In behandeling"";
                   default:
                       return ""Status onbekend"";
               }
           }
       }
   }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithIfInPropertyGetter_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
    class MyClass
    {
        public string BookingStatus
        {
            get
            {
                if(true)
                {
                    return ""lala"";
                }
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithWhileInPropertyGetter_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
    class MyClass
    {
        public string BookingStatus
        {
            get
            {
                while(5 == 5)
                {
                    return ""lala"";
                }
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithDoWhileInPropertyGetter_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
    class MyClass
    {
        public string BookingStatus
        {
            get
            {
                do
                {
                    return ""lala"";
                } while(5 == 5);
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithIfInPropertyGetter_WithoutBlock_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
    class MyClass
    {
        public string BookingStatus
        {
            get
            {
                if(true)
                    return ""lala"";
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithDoWhileInPropertyGetter_WithoutBlock_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
 
namespace ConsoleApplication1
{
    class MyClass
    {
        public string BookingStatus
        {
            get
            {
                do
                    return ""lala"";
                while(5 == 5);
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithDoWhileInMethodBody_WithoutBlock_DoesNotInvokeWarning()
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
            do
                Console.WriteLine();
            while (5 == 5);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithIfInMethodBody_WithoutBlock_DoesNotInvokeWarning()
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
            if(5 == 5)
                Console.WriteLine();
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithWhileInMethodBody_WithoutBlock_DoesNotInvokeWarning()
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
            while(5 == 5)
                Console.WriteLine();
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithUsingInMethodBody_WithoutBlock_DoesNotInvokeWarning()
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
            using(null as IDisposable)
                Console.WriteLine();
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithVoidMethod_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void MyMethod()
        {
            Console.WriteLine();
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
        void MyMethod() => Console.WriteLine();
    }
}";

            VerifyDiagnostic(original, string.Format(SimplifyExpressionBodiedMemberAnalyzer.Rule.MessageFormat.ToString(), "Method", "MyMethod"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void SimplifyExpressionBodiedMember_WithVoidMethod_ThrowingException_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void MyMethod()
        {
            throw new NotImplementedException();
        }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}