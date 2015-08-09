using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ExplicitAccessModifiers;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ExplicitAccessModifiersTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ExplicitAccessModifiersAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ExplicitAccessModifiersCodeFix();

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static class MyClass
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal class MyClass
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    class MyClass
    {
        public void Method() { }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal class MyClass
    {
        public void Method() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_StructDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal struct MyStruct
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_StructDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static struct MyStruct
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal struct MyStruct
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_StructDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public struct MyStruct
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_StructDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    struct MyStruct
    {
        public int Position;
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal struct MyStruct
    {
        public int Position;
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EnumDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal enum MyEnum
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EnumDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static enum MyEnum
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal enum MyEnum
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EnumDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public enum MyEnum
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EnumDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    enum MyEnum
    {
        Foo, Bar, Baz, Biz
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal enum MyEnum
    {
        Foo, Bar, Baz, Biz
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_DelegateDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    delegate void Foo(int bar);
}";

            var result = @"
namespace ConsoleApplication1
{
    internal delegate void Foo(int bar);
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_DelegateDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static delegate void Foo(int bar);
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal delegate void Foo(int bar);
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_DelegateDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public delegate void Foo(int bar);
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_DelegateDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    delegate void Foo(int bar);
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal delegate void Foo(int bar);
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_InterfaceDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    interface IMyInterface
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal interface IMyInterface
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_InterfaceDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static interface IMyInterface
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal interface IMyInterface
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_InterfaceDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public interface IMyInterface
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_InterfaceDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    interface IMyInterface
    {
        public int Position();
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal interface IMyInterface
    {
        public int Position();
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedClassDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        class MyInternalClass
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private class MyInternalClass
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedClassDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        static class MyInternalClass
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static private class MyInternalClass
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedClassDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private class MyInternalClass
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedClassDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    class MyClass
    {
        [Obsolete]
        class MyInternalClass
        {
            public void Method();
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal class MyClass
    {
        [Obsolete]
        private class MyInternalClass
        {
            public void Method();
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedStructDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        struct MyInternalStruct
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private struct MyInternalStruct
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedStructDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        static struct MyInternalStruct
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static private struct MyInternalStruct
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedStructDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private struct MyInternalStruct
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedStructDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    class MyClass
    {
        [Obsolete]
        struct MyInternalStruct
        {
            public void Method();
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal class MyClass
    {
        [Obsolete]
        private struct MyInternalStruct
        {
            public void Method();
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedEnumDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        enum MyInternalEnum
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private enum MyInternalEnum
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedEnumDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        static enum MyInternalEnum
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static private enum MyInternalEnum
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedEnumDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private enum MyInternalEnum
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedEnumDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    class MyClass
    {
        [Obsolete]
        enum MyInternalEnum
        {
            Foo, Bar, Baz, Biz
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal class MyClass
    {
        [Obsolete]
        private enum MyInternalEnum
        {
            Foo, Bar, Baz, Biz
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class Program
    {
        delegate void Foo(int bar);
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class Program
    {
        private delegate void Foo(int bar);
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class Program
    {
        static delegate void Foo(int bar);
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class Program
    {
        static private delegate void Foo(int bar);
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class Program
    {
        public delegate void Foo(int bar);
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    internal class Program
    {
        [Obsolete]
        delegate void Foo(int bar);
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    internal class Program
    {
        [Obsolete]
        private delegate void Foo(int bar);
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        interface MyInternalInterface
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private interface MyInternalInterface
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        static interface MyInternalInterface
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static private interface MyInternalInterface
        {
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private interface MyInternalInterface
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    class MyClass
    {
        [Obsolete]
        interface MyInternalInterface
        {
            public int Buzz();
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal class MyClass
    {
        [Obsolete]
        private interface MyInternalInterface
        {
            public int Buzz();
        }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_FieldDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        int Foo;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private int Foo;
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_FieldDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static class MyClass
    {
        static int Foo;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal class MyClass
    {
        static private int Foo;
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_FieldDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public int Foo;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_FieldDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        int Foo = 9;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private int Foo = 9;
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_PropertyDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        int Foo { get; set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private int Foo { get; set; }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_PropertyDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static class MyClass
    {
        static int Foo { get; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal class MyClass
    {
        static private int Foo { get; }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_PropertyDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public int Foo { get; set; }
    }
}";
            
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_PropertyDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        [Obsolete]
        int Foo { set; }    // I know this is bad, but we might as well test it
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        [Obsolete]
        private int Foo { set; }    // I know this is bad, but we might as well test it
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_MethodDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        void Foo() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private void Foo() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_MethodDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static void Foo() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static private void Foo() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_MethodDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        public void Foo() { }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_MethodDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        [Obsolete]
        void Foo()
        {
            var keepMe = true;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        [Obsolete]
        private void Foo()
        {
            var keepMe = true;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        MyClass() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private MyClass() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_StaticCtorMostNotHaveAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static MyClass() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        static MyClass() { }
    }
}";

            VerifyDiagnostic(original);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        public MyClass() { }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        [Obsolete]
        MyClass()
        {
            var keepMe = true;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        [Obsolete]
        private MyClass()
        {
            var keepMe = true;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_ClassHasExplicitNonDefaultModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        MyClass() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
        private MyClass() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventFieldDeclaration_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        event EventHandler MyEvent;
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        private event EventHandler MyEvent;
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventFieldDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        static event EventHandler MyEvent;
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        static private event EventHandler MyEvent;
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventFieldDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public event EventHandler MyEvent;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventFieldDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        [Obsolete]
        event EventHandler<int> MyEvent;
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        [Obsolete]
        private event EventHandler<int> MyEvent;
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventDeclaration_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        private event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        static event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        static private event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        [Obsolete]
        event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        [Obsolete]
        private event EventHandler MyEvent
        {
            add { var foo = value; }
            remove { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_IndexerDeclaration_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        int this[int index]
        {
            get { return index; }
            set { var foo = value; }
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        private int this[int index]
        {
            get { return index; }
            set { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_IndexerDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        protected int this[int index]
        {
            get { return index; }
            set { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_IndexerDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        [Obsolete]
        int this[int index]
        {
            get { return index; }
            set { var foo = value; }
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        [Obsolete]
        private int this[int index]
        {
            get { return index; }
            set { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }
    }
}