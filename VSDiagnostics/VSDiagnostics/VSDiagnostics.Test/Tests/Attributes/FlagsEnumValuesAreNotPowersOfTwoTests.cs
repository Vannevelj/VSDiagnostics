using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.FlagsEnumValuesAreNotPowersOfTwo;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class FlagsEnumValuesAreNotPowersOfTwoTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new FlagsEnumValuesAreNotPowersOfTwoAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new FlagsEnumValuesAreNotPowersOfTwoCodeFix();

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_HexValues()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_HexValues()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_NegativeValues()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_NoValues()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeShort()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeShort()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUshort()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUshort()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeInt()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUint()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUint()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeLong()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeLong()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUlong()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BaseTypeUlong()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_NotFlagsEnum()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_FlagsEnum_WithSystemNamespace()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt16()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUInt16()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt32()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUInt32()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeInt64()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeUInt64()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesArePowersOfTwo_BitShifting()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_ValuesOfOtherFlags()
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
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_CharactersInsteadOfIntValues()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BinaryExpressionsWithAllIdentifiersAreLeft()
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Days"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BitshiftedValuesNotPowersOfTwo()
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
        Monday = 75 << 1,
        WorkweekStart = Monday,
        Tuesday = 75 << 2,
        Wednesday = 75 << 3,
        Thursday = 75 << 4,
        Friday = 75 << 5,
        WorkweekEnd = Friday,
        Saturday = 75 << 6,
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

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Days"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BitOredValuesNotPowersOfTwo()
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
        WorkweekEnd = Friday | 63,
        Saturday = 1 << 6,
        Weekend = Saturday | Sunday,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_EnsuresFixLooksNice()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    internal enum CalendarType { Camp, Activity }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    internal enum CalendarType
    {
        Camp = 0,
        Activity = 1
    }
}";

            VerifyDiagnostic(original,
                string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "CalendarType"));
            VerifyFix(original, result);

        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_WithoutExplicitValues()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        A,
        B,
        C
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo
    {
        A = 0,
        B = 1,
        C = 2
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeByte()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : byte
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
    enum Foo : byte
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ValuesAreNotPowersOfTwo_BaseTypeSByte()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : sbyte
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
    enum Foo : sbyte
    {
        Bar = 0,
        Biz = 1,
        Baz = 2,
        Buz = 4,
        Boz = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.DefaultRule.MessageFormat.ToString(), "Foo"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_Byte_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : byte
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "byte"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_SByte_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : sbyte
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "sbyte"));
        }
    }
}