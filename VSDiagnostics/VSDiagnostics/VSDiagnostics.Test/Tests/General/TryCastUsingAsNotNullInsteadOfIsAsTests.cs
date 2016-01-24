using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TryCastUsingAsNotNullInsteadOfIsAsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new TryCastWithoutUsingAsNotNullAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new TryCastWithoutUsingAsNotNullCodeFix();

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndReferenceType()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndValueType()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndObjectIsUsedBeforeIs()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            Console.Write(o.GetType());
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            Console.Write(o.GetType());
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndObjectIsMethodParameter()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndElseClause()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleIrrelevantCasts()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_ReferenceType()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""test"";
            if (o is string)
            {
                var oAsString = (string) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""test"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_Struct()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_NullableStruct()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int?) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithoutCorrespondingCast()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            object irrelevant = 10.0;
            if (o is int)
            {
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ChainedVariableDeclaration()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                int? oAsInt = o as int?, x = 10;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                int? x = 10;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_TryCastNullCheck()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            int? oAsInt = o as int?;
            if (oAsInt != null)
            {

            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleRelevantCasts()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
                string anotherString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"),
                string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleIfConditions()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            if (o is string && 1 == 1)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null && 1 == 1)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ReferencingParameter()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_AsCastInIfCondition()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string && (o as string).Length > 1)
            {
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null && (oAsString).Length > 1)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCastInIfCondition()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string && ((string) o).Length > 1)
            {
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null && (oAsString).Length > 1)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastInSeparateExpression_ReferenceType()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(o as string);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null)
            {
                bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(oAsString);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastInSeparateExpression_ValueType()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                bool contains = new[] { 5, 6, 7 }.Contains((o as int?).Value);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                bool contains = new[] { 5, 6, 7 }.Contains((oAsInt32).Value);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastInSeparateExpression_ValueType_NullableCollectionElements()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                bool contains = new int?[] { 5, 6, 7 }.Contains(o as int?);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                bool contains = new int?[] { 5, 6, 7 }.Contains(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastToSelfdefinedType()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is Other)
            {
                Other myVar = o as Other;
            }
        }
    }

    class Other { }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsOther = o as Other;
            if (oAsOther != null)
            {
            }
        }
    }

    class Other { }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastUsingMethodReference()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (Get() is string)
            {
                string myVar = Get() as string;
            }
        }

        object Get()
        {
            return null;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_SplitVariableDefinitionAndAssignment()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            string myVar = null;
            if (o is string)
            {
                myVar = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            string myVar = null;
            var oAsString = o as string;
            if (oAsString != null)
            {
                myVar = oAsString;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DifferentTypes()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                var x = o as string;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_UsagesOfCastedVariable()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                var oAsInt = o as int?;

                Console.Write(oAsInt);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_UsagesOfCastedVariable()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int) o;

                Console.Write(oAsInt);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }
    }
}