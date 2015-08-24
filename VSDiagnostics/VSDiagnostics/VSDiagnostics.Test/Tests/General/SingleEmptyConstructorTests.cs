using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SingleEmptyConstructor;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SingleEmptyConstructorTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SingleEmptyConstructorAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new SingleEmptyConstructorCodeFix();

        [TestMethod]
        public void SingleEmptyConstructor_WithEmptyConstructor()
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

            VerifyDiagnostic(original, string.Format(SingleEmptyConstructorAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SingleEmptyConstructor_WithSingleLineCommentInConstructor()
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
        public void SingleEmptyConstructor_WithMultiLineCommentInConstructor()
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
        public void SingleEmptyConstructor_WithConstructorParameters()
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
        public void SingleEmptyConstructor_WithConstructorBody()
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
        public void SingleEmptyConstructor_WithImplicitPrivateConstructor()
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
        public void SingleEmptyConstructor_WithExplicitInternalConstructor()
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
        public void SingleEmptyConstructor_ConstructorHasAttributes()
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

        [TestMethod]
        public void SingleEmptyConstructor_ConstructorHasBaseCallWithArgument()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class MyExceptionClass : Exception
    {
        public MyExceptionClass() : base(""foo"")
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructor_ConstructorHasBaseCallWithoutArgument()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyExceptionClass : Exception
    {
        public MyExceptionClass() : base()
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    public class MyExceptionClass : Exception
    {
    }
}";

            VerifyDiagnostic(original, string.Format(SingleEmptyConstructorAnalyzer.Rule.MessageFormat.ToString(), "MyExceptionClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SingleEmptyConstructor_ConstructorHasThisCallWithArgument()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class MyExceptionClass : Exception
    {
        public MyExceptionClass() : this(""foo"")
        {
        }

        public MyExceptionClass(string s)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructor_ConstructorHasThisCallWithoutArgument()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyExceptionClass : Exception
    {
        public MyExceptionClass() : this()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructor_ConstructorHasXmlDocComment_ThisKeyword()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        /// <summary>
        /// Doc comment for Foo
        /// </summary>
        public Foo()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructor_ConstructorHasMultilineComment_ThisKeyword()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        /*
           Hi.  I'm a multiline comment.
        */
        public Foo()
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SingleEmptyConstructor_MultipleConstructors_ThisKeyword()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public Foo()
        {
        }

        public Foo(int i)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}