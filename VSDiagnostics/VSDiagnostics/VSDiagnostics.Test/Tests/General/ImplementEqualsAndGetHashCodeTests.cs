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
            if (obj == null || GetType() != obj.GetType())
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
            if (obj == null || GetType() != obj.GetType())
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
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo && _bar == value._bar && Foo == value.Foo && Bar == value.Bar;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^ _bar.GetHashCode() ^ Foo.GetHashCode() ^ Bar.GetHashCode();
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
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var value = (MyClass)obj;
            return _foo == value._foo && _bar == value._bar && Foo == value.Foo;
        }

        public override int GetHashCode()
        {
            return _foo.GetHashCode() ^ _bar.GetHashCode() ^ Foo.GetHashCode();
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
        public void ImplementEqualsAndGetHashCode_ImplementsEquals_HasField()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        string _foo = ""test"";

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
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
    }
}