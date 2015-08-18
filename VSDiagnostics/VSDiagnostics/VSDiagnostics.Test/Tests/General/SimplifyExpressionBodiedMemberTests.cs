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
        public void SimplifyExpressionBodiedMember_WithSimpleReturnMethod_InvokesWarning()
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
        public void SimplifyExpressionBodiedMember_WithSimpleGetterOnlyProperty_InvokesWarning()
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
        public void SimplifyExpressionBodiedMember_WithMultiLineGetterOnlyProperty_InvokesWarning()
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
        public void SimplifyExpressionBodiedMember_WithMultiStatementGetterOnlyProperty_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMultiStatementMethod_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMethodWithVoidReturn_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMethodWithoutReturn_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMethodWithExpressionBody_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithPropertyWithExpressionBody_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMethodWithUnreachableCode_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMethodAndTrailingComments_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithAutoImplementedGetAndSet_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithMultipleProperties_InvokesWarning()
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
        public void SimplifyExpressionBodiedMember_WithTrivia_AsProperty_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithTrivia_AsMethod_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithTrailingPropertyTrivia_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithAttribute_DoesNotInvokeWarning()
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
        public void SimplifyExpressionBodiedMember_WithGetAndSetImplementation_DoesNotInvokeWarning()
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
    }
}