using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.GetHashCodeRefersToMutableMember;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class GetHashCodeRefersToMutableMemberTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new GetHashCodeRefersToMutableMemberAnalyzer();

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_ConstantField()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        private const char Boo = '1';

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to const field Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_NonReadonlyField()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        private char _boo = '1';

        public override int GetHashCode()
        {
            return _boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to non-readonly field _boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_StaticField()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        private static readonly char _boo = '1';

        public override int GetHashCode()
        {
            return _boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to static field _boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_NonReadonlyStaticNonValueTypeField()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    public class Foo
    {
        private static Type _boo = typeof(Foo);

        public override int GetHashCode()
        {
            return _boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to static non-readonly non-value type, non-string field _boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_NonValueTypeNonStringField()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    public class Foo
    {
        private readonly Type _boo = typeof(Foo);

        public override int GetHashCode()
        {
            return _boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to non-value type, non-string field _boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_ImMutableMember_NoWarning()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    public class Foo
    {
        private readonly char _boo = '1';

        public override int GetHashCode()
        {
            return _boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_ImMutableStringMember_NoWarning()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    public class Foo
    {
        private readonly string _boo = ""1"";

        public override int GetHashCode()
        {
            return _boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_StaticProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public static char Boo { get; } = '1';

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to static property Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_NonValueTypeNonStringProperty()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    public class Foo
    {
        public Type Boo { get; } = typeof(Foo);

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to non-value type, non-string property Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_SettableProperty()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public char Boo { get; set; } = '1';

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to settable property Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_PropertyWithBodiedGetter()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public char Boo { get { return '1'; } }

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to property with bodied getter Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_StaticNonValueTypeSettablePropertyWithBodiedGetter()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    public class Foo
    {
        public static Type Boo { get { return typeof(Foo); } set { } }

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to static non-value type, non-string settable property with bodied getter Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_PropertyWithExpressionBodiedGetter()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public char Boo => '1';

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to property with bodied getter Boo");
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_ImmutableProperty_NoDiagnostic()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public char Boo { get; }

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_ImmutableStringProperty_NoDiagnostic()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public string Boo { get; }

        public override int GetHashCode()
        {
            return Boo.GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void GetHashCodeRefersToMutableMember_Method()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class Foo
    {
        public char Boo() { return '1'; }

        public override int GetHashCode()
        {
            return Boo().GetHashCode();
        }
    }
}";

            VerifyDiagnostic(original, "GetHashCode() refers to method Boo");
        }
    }
}
