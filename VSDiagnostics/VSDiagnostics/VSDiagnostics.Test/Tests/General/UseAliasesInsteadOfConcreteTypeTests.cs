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
        public void UseAliasesInsteadOfConcreteType_Int16BecomesShort_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int16Alias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32Alias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int64BecomesLong_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int64Alias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_UInt16BecomesUshort_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_UInt16Alias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_UInt32BecomesUint_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_UInt32Alias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_UInt64BecomesUlong_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_UInt64Alias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_ObjectBecomesObject_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_ObjectAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_ByteBecomesByte_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_ByteAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_SByteBecomesSbyte_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_SbyteAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_CharBecomesChar_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_CharAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_BooleanBecomesBool_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_BooleanAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_SingleBecomesFloat_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Single s = 1.5;
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
            float s = 1.5;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "float", "Single"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_SingleAlias_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            float s = 1.5;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DoubleBecomesDouble_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_DoubleAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_DecimalBecomesDecimal_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Decimal d = 1.5;
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
            decimal d = 1.5;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "decimal", "Decimal"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_DecimalAlias_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            decimal d = 1.5;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_StringBecomesString_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_StringAlias_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_UseVar_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_UserDefinedType_DoesNotInvokeWarning()
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
        public void UseAliasesInsteadOfConcreteType_SystemDotCharBecomesChar_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Method_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_ConversionOperator_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        public static explicit operator Int32(char c)
        {
            return c;
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        public static explicit operator int (char c)
        {
            return c;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(UseAliasesInsteadOfConcreteTypeAnalyzer.Rule.MessageFormat.ToString(), "int", "Int32"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Delegate_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Indexer_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Operator_InvokesWarning()
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
        public void UseAliasesInsteadOfConcreteType_Int32BecomesInt_Property_InvokesWarning()
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
    }
}