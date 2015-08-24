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
        public void ExplicitAccessModifiers_ClassDeclaration()
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
        public void ExplicitAccessModifiers_ClassDeclaration_ContainsNonAccessModifier()
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
    internal static class MyClass
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_ClassDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_StructDeclaration()
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
        public void ExplicitAccessModifiers_StructDeclaration_ContainsNonAccessModifier()
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
    internal static struct MyStruct
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_StructDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_StructDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_EnumDeclaration()
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
        public void ExplicitAccessModifiers_EnumDeclaration_ContainsNonAccessModifier()
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
    internal static enum MyEnum
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EnumDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_EnumDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_DelegateDeclaration()
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
        public void ExplicitAccessModifiers_DelegateDeclaration_ContainsNonAccessModifier()
        {
            var original = @"
namespace ConsoleApplication1
{
    static delegate void Foo(int bar);
}";

            var result = @"
namespace ConsoleApplication1
{
    internal static delegate void Foo(int bar);
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_DelegateDeclaration_ContainsAccessModifier()
        {
            var original = @"
namespace ConsoleApplication1
{
    public delegate void Foo(int bar);
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_DelegateDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_InterfaceDeclaration()
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
        public void ExplicitAccessModifiers_InterfaceDeclaration_ContainsNonAccessModifier()
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
    internal static interface IMyInterface
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_InterfaceDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_InterfaceDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_NestedClassDeclaration()
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
        public void ExplicitAccessModifiers_NestedClassDeclaration_ContainsNonAccessModifier()
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
        private static class MyInternalClass
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
        public void ExplicitAccessModifiers_NestedClassDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_NestedClassDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_NestedStructDeclaration()
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
        public void ExplicitAccessModifiers_NestedStructDeclaration_ContainsNonAccessModifier()
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
        private static struct MyInternalStruct
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
        public void ExplicitAccessModifiers_NestedStructDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_NestedStructDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_NestedEnumDeclaration()
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
        public void ExplicitAccessModifiers_NestedEnumDeclaration_ContainsNonAccessModifier()
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
        private static enum MyInternalEnum
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
        public void ExplicitAccessModifiers_NestedEnumDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_NestedEnumDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_NestedDelegateDeclaration()
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
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_ContainsNonAccessModifier()
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
        private static delegate void Foo(int bar);
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration()
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
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_ContainsNonAccessModifier()
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
        private static interface MyInternalInterface
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
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_NestedInterfaceDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_FieldDeclaration()
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
        public void ExplicitAccessModifiers_FieldDeclaration_ContainsNonAccessModifier()
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
    internal static class MyClass
    {
        private static int Foo;
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_FieldDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_FieldDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_PropertyDeclaration()
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
        public void ExplicitAccessModifiers_PropertyDeclaration_ContainsNonAccessModifier()
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
    internal static class MyClass
    {
        private static int Foo { get; }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_PropertyDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_PropertyDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_MethodDeclaration()
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
        public void ExplicitAccessModifiers_MethodDeclaration_ContainsNonAccessModifier()
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
        private static void Foo() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_MethodDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_MethodDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_ClassConstructorDeclaration()
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
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_StaticCtorMostNotHaveAccessModifier()
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
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_ClassHasExplicitNonDefaultModifier()
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
        public void ExplicitAccessModifiers_EventFieldDeclaration()
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
        public void ExplicitAccessModifiers_EventFieldDeclaration_ContainsNonAccessModifier()
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
        private static event EventHandler MyEvent;
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventFieldDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_EventFieldDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_EventDeclaration()
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
        public void ExplicitAccessModifiers_EventDeclaration_ContainsNonAccessModifier()
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
        private static event EventHandler MyEvent
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
        public void ExplicitAccessModifiers_EventDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_EventDeclaration_OnlyChangesAccessModifiers()
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
        public void ExplicitAccessModifiers_IndexerDeclaration()
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
        public void ExplicitAccessModifiers_IndexerDeclaration_ContainsAccessModifier()
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
        public void ExplicitAccessModifiers_IndexerDeclaration_OnlyChangesAccessModifiers()
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

        [TestMethod]
        public void ExplicitAccessModifiers_InterfaceMethodMemberDeclaration()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    internal interface MyInterface
    {
        void MyMethod();
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedInterfaceMethodMemberDeclaration()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private interface MyInternalInterface
        {
            void MyMethod();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_InterfacePropertyMemberDeclaration()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    internal interface MyInterface
    {
        int MyMethod { get; set; }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedInterfacePropertyMemberDeclaration()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        private interface MyInternalInterface
        {
            int MyMethod { get; set; }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    class MyClass
    {
        /// <summary>
        /// My XML doc for MyInternalClass...
        /// </summary>
        class MyInternalClass
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for MyInternalClass...
        /// </summary>
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
        public void ExplicitAccessModifiers_StructDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyStruct...
    /// Second line...
    /// </summary>
    struct MyStruct
    {
        /// <summary>
        /// My XML doc for MyInternalStruct...
        /// </summary>
        struct MyInternalStruct
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyStruct...
    /// Second line...
    /// </summary>
    internal struct MyStruct
    {
        /// <summary>
        /// My XML doc for MyInternalStruct...
        /// </summary>
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
        public void ExplicitAccessModifiers_InterfaceDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for IMyInterface...
    /// Second line...
    /// </summary>
    interface IMyInterface
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for IMyInterface...
    /// Second line...
    /// </summary>
    internal interface IMyInterface
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestednterfaceDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    class MyClass
    {
        /// <summary>
        /// My XML doc for MyInternalInterface...
        /// </summary>
        interface MyInternalInterface
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for MyInternalInterface...
        /// </summary>
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
        public void ExplicitAccessModifiers_EnumDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyEnum...
    /// Second line...
    /// </summary>
    enum MyEnum
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyEnum...
    /// Second line...
    /// </summary>
    internal enum MyEnum
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedEnumDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    class MyClass
    {
        /// <summary>
        /// My XML doc for MyInternalEnum...
        /// </summary>
        enum MyInternalEnum
        {
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for MyInternalEnum...
        /// </summary>
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
        public void ExplicitAccessModifiers_DelegateDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for Foo...
    /// </summary>
    /// <param name=""bar""></param>
    delegate void Foo(int bar);
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for Foo...
    /// </summary>
    /// <param name=""bar""></param>
    internal delegate void Foo(int bar);
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_NestedDelegateDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        /// <param name=""bar""></param>
        delegate void Foo(int bar);
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    /// <summary>
    /// My XML doc for MyClass...
    /// Second line...
    /// </summary>
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        /// <param name=""bar""></param>
        private delegate void Foo(int bar);
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_FieldDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        int Foo;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        private int Foo;
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_PropertyDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        int Foo { get; set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        private int Foo { get; set; }
    }
}";

            VerifyDiagnostic(original,
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "internal"),
                string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_MethodDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        void Foo() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for Foo...
        /// </summary>
        private void Foo() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassConstructorDeclaration_WithXmlDoc()
        {
            var original = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for MyClass ctor...
        /// </summary>
        MyClass() { }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
        /// <summary>
        /// My XML doc for MyClass ctor...
        /// </summary>
        private MyClass() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventFieldDeclaration_WithXmlDoc()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        /// <summary>
        /// My XML doc for MyEvent...
        /// </summary>
        event EventHandler MyEvent;
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        /// <summary>
        /// My XML doc for MyEvent...
        /// </summary>
        private event EventHandler MyEvent;
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), "private"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_EventDeclaration_WithXmlDoc()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        /// <summary>
        /// My XML doc for MyEvent...
        /// </summary>
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
        /// <summary>
        /// My XML doc for MyEvent...
        /// </summary>
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
        public void ExplicitAccessModifiers_IndexerDeclaration_WithXmlDoc()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        /// <summary>
        /// My XML doc for indexer...
        /// </summary>
        /// <param name=""index""></param>
        /// <returns>Value at index.</returns>
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
        /// <summary>
        /// My XML doc for indexer...
        /// </summary>
        /// <param name=""index""></param>
        /// <returns>Value at index.</returns>
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