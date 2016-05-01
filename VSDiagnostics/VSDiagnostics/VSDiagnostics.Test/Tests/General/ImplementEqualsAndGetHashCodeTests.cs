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
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasField()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo = ""test"";
    }
}";

            var result = @"
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
        public void ImplementEqualsAndGetHashCode_ClassDoesNotImplementEither_HasProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string Foo { get; set; } = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string Foo { get; set; } = ""test"";

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
        string _foo = ""test"";
        string _bar = ""test"";

        string Foo { get; set; } = ""test"";
        string Bar { get; set; } = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo = ""test"";
        string _bar = ""test"";

        string Foo { get; set; } = ""test"";
        string Bar { get; set; } = ""test"";

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
        string _foo = ""test"";
        string _bar = ""test"";

        string Foo { get; set; } = ""test"";
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
        string _foo = ""test"";
        string _bar = ""test"";

        string Foo { get; set; } = ""test"";
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
                   _bar.GetHashCode() ^
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
        string _foo;
    }
}";

            var result = @"
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
        string Foo { get; set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        string Foo { get; set; }

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
        string _foo;
        string _bar;

        string Foo { get; set; }
        string Bar { get; set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    struct MyStruct
    {
        string _foo;
        string _bar;

        string Foo { get; set; }
        string Bar { get; set; }

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
        string _foo;
        string _bar;

        string Foo { get; set; }
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
        string _foo;
        string _bar;

        string Foo { get; set; }
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
                   _bar.GetHashCode() ^
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
        string _foo = ""test"", _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo = ""test"", _bar = ""test"";

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
        string _foo = ""test"";
        static string _bar = ""test"";
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        string _foo;
        static string _bar;
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    interface IStruct { }

    struct MyStruct : IStruct
    {
        string _foo;
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
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        string _foo = ""test"";
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
        public virtual string Bar { get; set; }
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        string _foo = ""test"";
        static string _bar = ""test"";
        public override string Bar { get; set; }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyBaseClass
    {
        public virtual string Bar { get; set; }
        public override bool Equals(object obj) => true;
    }

    class MyClass : MyBaseClass
    {
        string _foo = ""test"";
        static string _bar = ""test"";
        public override string Bar { get; set; }

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
    }
}