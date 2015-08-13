using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.FlagsEnumValuesAreNotPowersOfTwo;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class FlagsEnumValuesAreNotPowersOfTwoTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new FlagsEnumValuesAreNotPowersOfTwoAnalyzer();

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = 4
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_HexValues_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0x0,
        Biz = 0x1,
        Baz = 0x2,
        Buz = 0x3,
        Boz = 0x4
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_HexValues_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0x0,
        Biz = 0x1,
        Baz = 0x2,
        Buz = 0x4,
        Boz = 0x8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_NegativeValues_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0,
        Biz = -1,
        Baz = -2,
        Buz = -4,
        Boz = -8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_NoValues_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0x0,
        Biz,
        Baz,
        Buz,
        Boz
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeShort_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : short
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = short.MaxValue
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeShort_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : short
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUshort_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ushort
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = ushort.MaxValue
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUshort_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ushort
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : int
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = int.MaxValue
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeInt_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : int
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUint_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : uint
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = uint.MaxValue
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUint_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ulong
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeLong_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : long
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = long.MaxValue
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeLong_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : long
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUlong_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ulong
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = ulong.MaxValue
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUlong_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ulong
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_NotFlagsEnum_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = 4
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_FlagsEnum_WithSystemNamespace_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [System.Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = 4
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
        }

        // May be useful later
        /*
        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BitShifting_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1 << 0,
        Baz = 1 << 1,
        Buz = 1 << 2,
        Boz = 1 << 3
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_ValuesOfOtherFlags_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    [Flags]
    enum Days
    {
        None = 0,
        Sunday = 1,
        Monday = 1 << 1,
        WorkweekStart = Monday,
        Tuesday = 1 << 2,
        Wednesday = 1 << 3,
        Thursday = 1 << 4,
        Friday = 1 << 5,
        WorkweekEnd = Friday,
        Saturday = 1 << 6,
        Weekend = Saturday | Sunday,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday
    }
}";
            VerifyDiagnostic(original);
        }
        */
    }
}