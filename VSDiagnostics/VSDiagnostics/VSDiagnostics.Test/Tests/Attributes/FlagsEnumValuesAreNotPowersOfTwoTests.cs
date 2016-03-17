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
        J = 9
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
        I = 8
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "sbyte"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_int_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : int
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
	    L = 11,
	    M = 12,
	    N = 13,
	    O = 14,
	    P = 15,
	    Q = 16,
	    R = 17,
	    S = 18,
	    T = 19,
	    U = 20,
	    V = 21,
	    W = 22,
	    X = 23,
	    Y = 24,
	    Z = 25,
	    AA = 26,
	    BB = 27,
	    CC = 28,
	    DD = 29,
	    EE = 30,
	    FF = 31,
        GG = 32
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "int"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_uint_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : uint
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
	    L = 11,
	    M = 12,
	    N = 13,
	    O = 14,
	    P = 15,
	    Q = 16,
	    R = 17,
	    S = 18,
	    T = 19,
	    U = 20,
	    V = 21,
	    W = 22,
	    X = 23,
	    Y = 24,
	    Z = 25,
	    AA = 26,
	    BB = 27,
	    CC = 28,
	    DD = 29,
	    EE = 30,
	    FF = 31,
        GG = 32,
        HH = 33
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "uint"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_short_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : short
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
	    L = 11,
	    M = 12,
	    N = 13,
	    O = 14,
	    P = 15,
        Q = 16
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "short"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ushort_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ushort
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
	    L = 11,
	    M = 12,
	    N = 13,
	    O = 14,
	    P = 15,
        Q = 16,
        R = 17
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "ushort"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_long_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : long
    {
	    A = 0L,
	    B = 1L,
	    C = 1L << 1,
	    D = 1L << 2,
	    E = 1L << 3,
	    F = 1L << 4,
	    G = 1L << 5,
	    H = 1L << 6,
	    I = 1L << 7,
	    J = 1L << 8,
	    K = 1L << 9,
	    L = 1L << 10,
	    M = 1L << 11,
	    N = 1L << 12,
	    O = 1L << 13,
	    P = 1L << 14,
	    Q = 1L << 15,
	    R = 1L << 16,
	    S = 1L << 17,
	    T = 1L << 18,
	    U = 1L << 19,
	    V = 1L << 20,
	    W = 1L << 21,
	    X = 1L << 22,
	    Y = 1L << 23,
	    Z = 1L << 24,
	    AA = 1L << 25,
	    BB = 1L << 26,
	    CC = 1L << 27,
	    DD = 1L << 28,
	    EE = 1L << 29,
	    FF = 1L << 30,
	    GG = 1L << 31,
        HH = 1L << 32,
	    II = 1L << 33,
	    JJ = 1L << 34,
	    KK = 1L << 35,
	    LL = 1L << 36,
	    MM = 1L << 37,
	    NN = 1L << 38,
	    OO = 1L << 39,
	    PP = 1L << 40,
	    QQ = 1L << 41,
	    RR = 1L << 42,
	    SS = 1L << 43,
	    TT = 1L << 44,
	    UU = 1L << 45,
	    VV = 1L << 46,
	    WW = 1L << 47,
	    XX = 1L << 48,
	    YY = 1L << 49,
	    ZZ = 1L << 50,
	    AAA = 1L << 51,
	    BBB = 1L << 52,
	    CCC = 1L << 53,
	    DDD = 1L << 54,
	    EEE = 1L << 55,
	    FFF = 1L << 56,
	    GGG = 1L << 57,
	    HHH = 1L << 58,
	    III = 1L << 59,
	    JJJ = 1L << 60,
	    KKK = 1L << 61,
	    LLL = 1L << 62,
        MMM = 1L << 63
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "long"));
        }

        [TestMethod]
        public void FlagsEnumValuesAreNotPowersOfTwo_ulong_TooManyMembersForFlags()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Flags]
    enum Foo : ulong
    {
		A = 0UL,
		B = 1UL,
		C = 1UL << 1,
		D = 1UL << 2,
		E = 1UL << 3,
		F = 1UL << 4,
		G = 1UL << 5,
		H = 1UL << 6,
		I = 1UL << 7,
		J = 1UL << 8,
		K = 1UL << 9,
		L = 1UL << 10,
		M = 1UL << 11,
		N = 1UL << 12,
		O = 1UL << 13,
		P = 1UL << 14,
		Q = 1UL << 15,
		R = 1UL << 16,
		S = 1UL << 17,
		T = 1UL << 18,
		U = 1UL << 19,
		V = 1UL << 20,
		W = 1UL << 21,
		X = 1UL << 22,
		Y = 1UL << 23,
		Z = 1UL << 24,
		AA = 1UL << 25,
		BB = 1UL << 26,
		CC = 1UL << 27,
		DD = 1UL << 28,
		EE = 1UL << 29,
		FF = 1UL << 30,
		GG = 1UL << 31,
	    HH = 1UL << 32,
		II = 1UL << 33,
		JJ = 1UL << 34,
		KK = 1UL << 35,
		LL = 1UL << 36,
		MM = 1UL << 37,
		NN = 1UL << 38,
		OO = 1UL << 39,
		PP = 1UL << 40,
		QQ = 1UL << 41,
		RR = 1UL << 42,
		SS = 1UL << 43,
		TT = 1UL << 44,
		UU = 1UL << 45,
		VV = 1UL << 46,
		WW = 1UL << 47,
		XX = 1UL << 48,
		YY = 1UL << 49,
		ZZ = 1UL << 50,
		AAA = 1UL << 51,
		BBB = 1UL << 52,
		CCC = 1UL << 53,
		DDD = 1UL << 54,
		EEE = 1UL << 55,
		FFF = 1UL << 56,
		GGG = 1UL << 57,
		HHH = 1UL << 58,
		III = 1UL << 59,
		JJJ = 1UL << 60,
		KKK = 1UL << 61,
		LLL = 1UL << 62,
		MMM = 1UL << 63,
        NNN = 1UL << 64
    }
}";

            VerifyDiagnostic(original, string.Format(FlagsEnumValuesAreNotPowersOfTwoAnalyzer.ValuesDontFitRule.MessageFormat.ToString(), "Foo", "ulong"));
        }
    }
}