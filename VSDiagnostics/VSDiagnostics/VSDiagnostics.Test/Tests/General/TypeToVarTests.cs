using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.TypeToVar;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TypeToVarTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new TypeToVarAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new TypeToVarCodeFix();

        [TestMethod]
        public void TypeToVar_WithLocalPredefinedType()
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
            int x = 0;
        }
    }
}";
            var newSource = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var x = 0;
        }
    }
}";

            VerifyDiagnostic(original, TypeToVarAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, newSource);
        }

        [TestMethod]
        public void TypeToVar_WithLocalPredefinedTypeInitializedFromMethod()
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
            int x = GetInt();
        }

        int GetInt()
        {
            return 0;
        }
    }
}";
            var newSource = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var x = GetInt();
        }

        int GetInt()
        {
            return 0;
        }
    }
}";

            VerifyDiagnostic(original, TypeToVarAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, newSource);
        }

        [TestMethod]
        public void TypeToVar_WithLocalDefinedWithVar()
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
            var x = 0;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLocalAssignedUsingExplicitConversion()
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
            int x = (int)0.0;
        }
    }
}";

            var newSource = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var x = (int)0.0;
        }
    }
}";

            VerifyDiagnostic(original, TypeToVarAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, newSource);
        }

        [TestMethod]
        public void TypeToVar_WithLocalAssignedUsingImplicitConversion()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            IEnumerable<string> x = new List<string>();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithFieldDeclaration()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        int x = 0;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLocalUninitialized()
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
            int x;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLocalBaseClassTypeInitializedWithDerivedType()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        Base x = new Derived();
    }

    class Base
    {
    }

    class Derived : Base
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLocalConstPredefinedType()
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
            const int x = 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLambdaExpression()
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
            Action myAction = () => Console.WriteLine(""Hello"");
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLambdaExpression_NoType()
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
            Action x = () => { };
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVar_WithLocalDefinedWithDynamic()
        {
            var original = @"
using System.Dynamic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            dynamic exceptionInfo = new ExpandoObject();
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}