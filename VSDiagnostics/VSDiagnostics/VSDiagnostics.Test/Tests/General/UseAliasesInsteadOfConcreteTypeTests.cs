using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.UseAliasesInsteadOfConcreteType;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class UseAliasesInsteadOfConcreteTypeTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new UseAliasesInsteadOfConcreteTypeAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new UseAliasesInsteadOfConcreteTypeCodeFix();

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int16BecomesShort()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Int16 i16 = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            short i16 = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "short", "Int16"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int16Alias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            short i16 = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Int32 i32 = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int i32 = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32Alias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int i32 = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int64BecomesLong()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Int64 i64 = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            long i64 = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "long", "Int64"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int64Alias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            long i64 = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UInt16BecomesUshort()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            UInt16 ui16 = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            ushort ui16 = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "ushort", "UInt16"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UInt16Alias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            ushort ui16 = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UInt32BecomesUint()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            UInt32 ui32 = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            uint ui32 = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "uint", "UInt32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UInt32Alias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            uint ui32 = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UInt64BecomesUlong()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            UInt64 ui64 = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            ulong ui64 = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "ulong", "UInt64"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UInt64Alias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            ulong ui64 = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_ObjectBecomesObject()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Object o = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "object", "Object"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_ObjectAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_ByteBecomesByte()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Byte b = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            byte b = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "byte", "Byte"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_ByteAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            byte b = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_SByteBecomesSbyte()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            SByte sb = 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            sbyte sb = 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "sbyte", "SByte"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_SbyteAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            sbyte sb = 9;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_CharBecomesChar()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Char c = 'r';
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            char c = 'r';
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "char", "Char"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_CharAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            char c = 'r';
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_BooleanBecomesBool()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Boolean b = true;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool b = true;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "bool", "Boolean"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_BooleanAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool b = true;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_SingleBecomesFloat()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Single s = 1.5f;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            float s = 1.5f;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "float", "Single"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_SingleAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            float s = 1.5f;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DoubleBecomesDouble()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Double d = 1.5;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            double d = 1.5;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "double", "Double"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DoubleAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            double d = 1.5;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DecimalBecomesDecimal()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Decimal d = 1.5m;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            decimal d = 1.5m;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "decimal", "Decimal"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DecimalAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            decimal d = 1.5m;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_StringBecomesString()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            String s = ""hi"";
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            string s = ""hi"";
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "string", "String"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_StringAlias()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            string s = ""hi"";
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UseVar()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var s = ""hi"";
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_UserDefinedType()
        {
            var original = @"
namespace ConsoleApplication1
{
    struct Char { }

    class MyClass
    {
        void Method()
        {
            Char c;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_SystemDotCharBecomesChar()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            System.Char c = 'r';
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            char c = 'r';
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "char", "Char"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Method()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        Int32 Method()
        {
            return 9;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        int Method()
        {
            return 9;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_ConversionOperator()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        public static explicit operator Int32(MyClass c)
        {
            return 5;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        public static explicit operator int (MyClass c)
        {
            return 5;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Delegate()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    delegate Int32 Foo(char bar);
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    delegate int Foo(char bar);
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Indexer()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        Int32 this[int index]
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
        int this[int index]
        {
            get { return index; }
            set { var foo = value; }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Operator()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public static Int32 operator +(char c, Program p)
        {
            return c;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public static int operator +(char c, Program p)
        {
            return c;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Property()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public Int32 Foo { get; set; }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public int Foo { get; set; }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }


        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Int32DotMaxValue()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo()
        {
            var goo = Int32.MaxValue;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo()
        {
            var goo = int.MaxValue;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_CharBecomesChar_CharDotIsWhiteSpace()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo()
        {
            var goo = Char.IsWhiteSpace(' ');
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo()
        {
            var goo = char.IsWhiteSpace(' ');
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "char", "Char"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Nameof()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var a = nameof(Int16);
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Typeof()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo()
        {
            var x = typeof(Int16);
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo()
        {
            var x = typeof(short);
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "short", "Int16"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Parameter()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo(Int32 i)
        {
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program
    {
        public void Foo(int i)
        {
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Parameter_WithAlias()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(int i)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_TypeArgument()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program<T>
    {
        public void Foo()
        {
            new Program<Int32>();
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    public class Program<T>
    {
        public void Foo()
        {
            new Program<int>();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_TypeArgument_WithAlias()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    public class Program<T>
    {
        public void Foo()
        {
            new Program<int>();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_ParenthesizedExpression()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int x = (Int32.Parse(""5""));
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int x = (int.Parse(""5""));
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DoesNotSimplifyCasts()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int x = (int)Int32.Parse(""5"");
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int x = (int)int.Parse(""5"");
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_MultipleGenericTypeParameters()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass<T, U>
    {
        void Method()
        {
            new MyClass<Int32, int>();
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass<T, U>
    {
        void Method()
        {
            new MyClass<int, int>();
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_WithSurroundingTrivia()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            string s = string.Format(
                            ""{0}"",
                            String.Format(""{0}"", DateTime.Now)
            );
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            string s = string.Format(
                            ""{0}"",
                            string.Format(""{0}"", DateTime.Now)
            );
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "string", "String"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }
    }
}