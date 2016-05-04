using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ImplementEqualsAndGetHashCode;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ImplementEqualsAndGetHashCodeTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ImplementEqualsAndGetHashCodeAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new ImplementEqualsAndGetHashCodeCodeFix();

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasReadonlyField()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasGetOnlyProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string Foo { get; } = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string Foo { get; } = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return Foo == value.Foo;
        }

        public override int GetHashCode()
        {
            return Foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasMultipleFieldsAndProperties()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        readonly string _bar = ""test"";

        string Foo { get; } = ""test"";
        string Bar { get; } = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        readonly string _bar = ""test"";

        string Foo { get; } = ""test"";
        string Bar { get; } = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo &&
                   _bar == value._bar &&
                   Foo == value.Foo &&
                   Bar == value.Bar;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   _bar.GetHashCode() ^
                   Foo.GetHashCode() ^
                   Bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasSetOnlyProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        string _bar = ""test"";

        string Foo { get; } = ""test"";
        string Bar
        {
            set { _bar = value; }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        string _bar = ""test"";

        string Foo { get; } = ""test"";
        string Bar
        {
            set { _bar = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo &&
                   _bar == value._bar &&
                   Foo == value.Foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   Foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_DoesNotHaveFieldOrProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassImplementsEquals_HasField()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructDoesNotImplementEither_HasField()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        readonly string _foo;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        readonly string _foo;

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyStruct) != obj.GetType())
            {
                return false;
            }

            var value = (MyStruct)obj;
            return _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyStruct"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructDoesNotImplementEither_HasProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        string Foo { get; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        string Foo { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyStruct) != obj.GetType())
            {
                return false;
            }

            var value = (MyStruct)obj;
            return Foo == value.Foo;
        }

        public override int GetHashCode()
        {
            return Foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyStruct"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructDoesNotImplementEither_HasMultipleFieldsAndProperties()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        readonly string _foo;
        readonly string _bar;

        string Foo { get; }
        string Bar { get; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        readonly string _foo;
        readonly string _bar;

        string Foo { get; }
        string Bar { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyStruct) != obj.GetType())
            {
                return false;
            }

            var value = (MyStruct)obj;
            return _foo == value._foo &&
                   _bar == value._bar &&
                   Foo == value.Foo &&
                   Bar == value.Bar;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   _bar.GetHashCode() ^
                   Foo.GetHashCode() ^
                   Bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyStruct"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructDoesNotImplementEither_HasSetOnlyProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        readonly string _foo;
        string _bar;

        string Foo { get; }
        string Bar
        {
            set { _bar = value; }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        readonly string _foo;
        string _bar;

        string Foo { get; }
        string Bar
        {
            set { _bar = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyStruct) != obj.GetType())
            {
                return false;
            }

            var value = (MyStruct)obj;
            return _foo == value._foo &&
                   _bar == value._bar &&
                   Foo == value.Foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   Foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyStruct"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructDoesNotImplementEither_DoesNotHaveFieldOrProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructImplementsEquals_HasField()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        string _foo;

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyStruct) != obj.GetType())
            {
                return false;
            }

            var value = (MyStruct)obj;
            return _foo == value._foo;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasChainedFields()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"", _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"", _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo &&
                   _bar == value._bar;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   _bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasStaticFields()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }
        
        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasBaseClassImplementingEquals()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return base.Equals(obj) &&
                   _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasBaseClassImplementingEquals_HasInterface()
        {
            var original = @"
namespace ConsoleApplication1
{
    interface IClass { }

    class MyBaseClass
    {
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass, IClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    interface IClass { }

    class MyBaseClass
    {
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass, IClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return base.Equals(obj) &&
                   _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_StructDoesNotImplementEither_ImplementsInterface()
        {
            var original = @"
namespace ConsoleApplication1
{
    interface IStruct { }

    struct MyStruct : IStruct
    {
        readonly string _foo;
        static string _bar;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    interface IStruct { }

    struct MyStruct : IStruct
    {
        readonly string _foo;
        static string _bar;

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyStruct) != obj.GetType())
            {
                return false;
            }

            var value = (MyStruct)obj;
            return _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyStruct"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasBaseClassImplementingEquals_BaseClassHasField()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public string foo;
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public string foo;
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return base.Equals(obj) &&
                   _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasBaseClassNotImplementingEquals()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public string foo;
        public override int GetHashCode() => 1;  // disable analyzer for this
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public string foo;
        public override int GetHashCode() => 1;  // disable analyzer for this
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesImplementsEquals_OverridesPropertyInBaseClass()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public virtual string Bar { get; }
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
        public override string Bar { get; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public virtual string Bar { get; }
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        readonly string _foo = ""test"";
        static string _bar = ""test"";
        public override string Bar { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return base.Equals(obj) &&
                   _foo == value._foo &&
                   Bar == value.Bar;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   Bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_EqualsComparesAll_GetHashCodeUsesReadonly()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        readonly string _foo = ""test"";
        string _bar = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo &&
                   _bar == value._bar;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_EqualsComparesAll_GetHashCodeUsesValueTypes()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct { }

    class MyClass
    {
        string _foo = ""test"";
        MyStruct _bar;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct { }

    class MyClass
    {
        string _foo = ""test"";
        MyStruct _bar;

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo &&
                   _bar.Equals(value._bar);
        }

        public override int GetHashCode()
        {
            return _bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_EqualsComparesAll_GetHashCodeUsesGetOnlyProperties()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo { get; set; }
        string _bar { get; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo { get; set; }
        string _bar { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo &&
                   _bar == value._bar;
        }

        public override int GetHashCode()
        {
            return _bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_EqualsUsesEqualsOnValueTypes()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct MyStruct { }

    class MyClass
    {
        MyStruct _foo;
        MyStruct _bar { get; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct { }

    class MyClass
    {
        MyStruct _foo;
        MyStruct _bar { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(MyClass) != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo.Equals(value._foo) &&
                   _bar.Equals(value._bar);
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^
                   _bar.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ImplementEqualsAndGetHashCodeAnalyzer.Rule.MessageFormat.ToString(), "MyClass"));
            VerifyFix(original, result);
        }
    }
}