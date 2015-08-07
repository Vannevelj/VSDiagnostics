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
        public void TypeToVar_WithLocalPredefinedType_InvokesWarning()
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
        public void TypeToVar_WithLocalPredefinedTypeInitializedFromMethod_InvokesWarning()
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
        public void TypeToVar_WithLocalDefinedWithVar_DoesNotInvokeWarning()
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
        public void TypeToVar_WithLocalAssignedUsingExplicitConversion_InvokesWarning()
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
        public void TypeToVar_WithLocalAssignedUsingImplicitConversion_DoesNotInvokeWarning()
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
        public void TypeToVar_WithFieldDeclaration_DoesNotInvokeWarning()
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
        public void TypeToVar_WithLocalUninitialized_DoesNotInvokeWarning()
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
        public void TypeToVar_WithLocalBaseClassTypeInitializedWithDerivedType_DoesNotInvokeWarning()
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
    }
}