﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SingleEmptyConstructor;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SingleEmptyConstructorAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SingleEmptyConstructorAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new SingleEmptyConstructorCodeFix();

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithEmptyConstructor_InvokesWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public MyClass()
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
    }
}";
            
            VerifyDiagnostic (original, string.Format(SingleEmptyConstructorAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithSingleLineCommentInConstructor_DoesNotInvokeWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public MyClass()
        {
            // ctor has comment
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithMultiLineCommentInConstructor_DoesNotInvokeWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public MyClass()
        {
            /* ctor has comment
               ctor has multiline comment */
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithConstructorParameters_DoesNotInvokeWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public MyClass(int foo)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithConstructorBody_DoesNotInvokeWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public int Foo { get; }

        public MyClass()
        {
            Foo = 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithImplicitPrivateConstructor_DoesNotInvokeWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public int Foo { get; }

        MyClass()
        {
            Foo = 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_WithExplicitInternalConstructor_DoesNotInvokeWarning ()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public int Foo { get; }

        internal MyClass()
        {
            Foo = 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructorAnalyzer_ConstructorHasAttributes_DoesNotInvokeWarning ()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class MyClass
    {
        [Obsolete]
        public MyClass()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}