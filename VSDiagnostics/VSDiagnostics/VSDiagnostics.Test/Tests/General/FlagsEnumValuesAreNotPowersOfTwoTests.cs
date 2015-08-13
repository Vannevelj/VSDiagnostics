using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.FlagsEnumValuesAreNotPowersOfTwo;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class FlagsEnumValuesAreNotPowersOfTwoTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new FlagsEnumValuesAreNotPowersOfTwoAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new FlagsEnumValuesAreNotPowersOfTwoCodeFix();

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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_NegativeValues_InvokesWarning()
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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : uint
    {
        Bar = 0U,
        Biz = 1U,
        Baz = 2U,
        Buz = 4U,
        Boz = 8U
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUint_DoesNotInvokeWarning()
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

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : long
    {
        Bar = 0L,
        Biz = 1L,
        Baz = 2L,
        Buz = 4L,
        Boz = 8L
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ulong
    {
        Bar = 0UL,
        Biz = 1UL,
        Baz = 2UL,
        Buz = 4UL,
        Boz = 8UL
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
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

            var result = @"
using System;

namespace ConsoleApplication1
{
    [System.Flags]
    enum Foo
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt16_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : Int16
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = short.MaxValue
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : Int16
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUInt16_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : UInt16
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = ushort.MaxValue
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : UInt16
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt32_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : Int32
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = int.MaxValue
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : Int32
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUInt32_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : UInt32
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = uint.MaxValue
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : UInt32
    {
        Bar = 0U,
        Biz = 1U,
        Baz = 2U,
        Buz = 4U,
        Boz = 8U
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt64_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : Int64
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = long.MaxValue
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : Int64
    {
        Bar = 0L,
        Biz = 1L,
        Baz = 2L,
        Buz = 4L,
        Boz = 8L
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUInt64_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : UInt64
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 3,
        Boz = ulong.MaxValue
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : UInt64
    {
        Bar = 0UL,
        Biz = 1UL,
        Baz = 2UL,
        Buz = 4UL,
        Boz = 8UL
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BitShifting_DoesNotInvokeWarning()
        {
            var original = @"
using System;

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
using System;

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

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_CharactersInsteadOfIntValues_DoesNotInvokeWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        Bar = 'a',
        Biz = 'b',
        Baz = 'c',
        Buz = 'd',
        Boz = 'e'
    }
}";

            var result = @"
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BinaryExpressionsWithAllIdentifiersAreLeft_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Days
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        WorkweekStart = Monday,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        WorkweekEnd = Friday,
        Saturday = 7,
        Weekend = Saturday | Sunday,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Days
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        WorkweekStart = Monday,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        WorkweekEnd = Friday,
        Saturday = 64,
        Weekend = Saturday | Sunday,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.Rule.MessageFormat.ToString(), "Days"));
            VerifyFix(original, result);
        }
    }
}