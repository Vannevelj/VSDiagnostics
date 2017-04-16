using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.FieldCanBeReadonly;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class FieldCanBeReadonlyTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new FieldCanBeReadonlyAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new FieldCanBeReadonlyCodeFix();

        [TestMethod]
        public void FieldCanBeReadonly_AssignedInline()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo = 0;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private readonly int _foo = 0;
    }
}";

            VerifyDiagnostic(original, string.Format(FieldCanBeReadonlyAnalyzer.Rule.MessageFormat.ToString(), "_foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MultipleFieldsCanBeReadonly_AssignedInline()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo = 0, _bar = 1;
    }
}";
            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private readonly int _foo = 0, _bar = 1;
    }
}";

            VerifyDiagnostic(original,
                string.Format(FieldCanBeReadonlyAnalyzer.Rule.MessageFormat.ToString(), "_foo"),
                string.Format(FieldCanBeReadonlyAnalyzer.Rule.MessageFormat.ToString(), "_bar"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FieldCanBeReadonly_AssignedInCtor()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;

        internal MyClass()
        {
            _foo = 0;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private readonly int _foo;

        internal MyClass()
        {
            _foo = 0;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(FieldCanBeReadonlyAnalyzer.Rule.MessageFormat.ToString(), "_foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_AssignedInProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;

        int Foo
        {
            get { return _foo; }
            set { _foo = value; }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_AssignedInMethod()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;

        void SetFoo(int value)
        {
            _foo = value;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_AssignedInMethodWithCompundOperator()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo = 0;

        void SetFoo(int value)
        {
            _foo += value;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_PostfixIncrementedInMethod()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;
        void Meh()
        {
            _foo++;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_PrefixDecrementInMethod()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;
        void Meh()
        {
            --_foo;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_AssignedInMethodInPartialClass()
        {
            var original = @"
namespace ConsoleApplication1
{
    partial class MyClass
    {
        private int _foo;
    }

    partial class MyClass
    {
        void SetFoo()
        {
            _foo = 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_PassedAsOutParameter()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;

        void Foo()
        {
            int.TryParse(""123"", out _foo);
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_PassedAsRefParameter()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private int _foo;

        void Foo()
        {
            Bar(ref _foo);
        }

        void Bar(ref int foo)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_StaticFieldIsAssignedInStaticCtor()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private static int _foo;

        static MyClass()
        {
            _foo = 0;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private static readonly int _foo;

        static MyClass()
        {
            _foo = 0;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(FieldCanBeReadonlyAnalyzer.Rule.MessageFormat.ToString(), "_foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FieldCannotBeReadonly_StaticFieldIsAssignedInNonStaticCtor()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private static int _foo;

        MyClass()
        {
            _foo = 0;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldIsReadonly_AssignedInline()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        private readonly int _foo = 0;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FieldCanBeReadonly_AssignedInline_InStruct()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        private static int _foo = 0;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        private static readonly int _foo = 0;
    }
}";

            VerifyDiagnostic(original, string.Format(FieldCanBeReadonlyAnalyzer.Rule.MessageFormat.ToString(), "_foo"));
            VerifyFix(original, result);
        }
    }
}